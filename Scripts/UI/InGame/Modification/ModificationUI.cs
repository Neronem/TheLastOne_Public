using System;
using System.Collections.Generic;
using System.Linq;
using _1.Scripts.Entity.Scripts.Player.Core;
using _1.Scripts.Manager.Core;
using _1.Scripts.UI.Inventory;
using _1.Scripts.Util;
using _1.Scripts.Weapon.Scripts.Common;
using _1.Scripts.Weapon.Scripts.Grenade;
using _1.Scripts.Weapon.Scripts.Guns;
using _1.Scripts.Weapon.Scripts.Hack;
using _1.Scripts.Weapon.Scripts.Melee;
using _1.Scripts.Weapon.Scripts.WeaponDetails;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using UIManager = _1.Scripts.Manager.Subs.UIManager;

namespace _1.Scripts.UI.InGame.Modification
{
    public class ModificationUI : UIBase
    {
        private static readonly WeaponType[] SlotOrder = new[]
        {
            WeaponType.Rifle,
            WeaponType.Pistol,
            WeaponType.GrenadeLauncher,
            WeaponType.HackGun
        };

        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;
        
        [Header("Preview")] 
        [SerializeField] private Transform previewSpawnPoint;
        private Dictionary<WeaponType, GameObject> weaponPreviewMap;
        private PreviewWeaponHandler previewWeaponHandler;
        
        [Header("Part Button")]
        [SerializeField] private List<Button> partButtons;
        [SerializeField] private List<TextMeshProUGUI> partButtonTexts;
        [SerializeField] private List<Image> partButtonImages;
        private readonly PartType[] allPartTypes = { PartType.Sight, PartType.Sight, PartType.FlameArrester, PartType.Suppressor, PartType.Silencer, PartType.ExtendedMag };
        
        [Header("Weapon & Part Name")]
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI partNameText;
        [SerializeField] private TextMeshProUGUI partDescriptionText;
        
        [Header("Part Highlight Material")]
        [SerializeField] private Material partHighlightMaterial;
        
        [Header("Stat")]
        [SerializeField] private Slider damageSlider;
        [SerializeField] private Slider rpmSlider;
        [SerializeField] private Slider recoilSlider;
        [SerializeField] private Slider weightSlider;
        [SerializeField] private Slider ammoSlider;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI rpmText;
        [SerializeField] private TextMeshProUGUI recoilText;
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private TextMeshProUGUI ammoText;
        
        [Header("Required")]
        [SerializeField] private TextMeshProUGUI requiredText;
        
        [Header("Apply Modal")] 
        [SerializeField] private ModalWindowManager modalWindowManager;
        [SerializeField] private GameObject applyModal;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        [Header("Buttons")]
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        
        private Dictionary<WeaponType, BaseWeapon> ownedWeapons = new();
        private int currentWeaponIdx = 0;
        private WeaponType CurrentWeaponType => GetSlotWeaponType(currentWeaponIdx);
        private BaseWeapon CurrentWeapon => ownedWeapons.GetValueOrDefault(CurrentWeaponType);
        private Dictionary<(PartType, int), WeaponPartData> partDataMap;
        private WeaponPartData selectedPartData;

        private PartType? selectedPartType;
        private int selectedPartId;

        private PlayerCondition playerCondition;
        private PlayerWeapon playerWeapon;

        private PartType? lastHighlightedPartType;
        private Material lastSelectedPartMaterial = null;

        public override void Initialize(UIManager manager, object param = null)
        {
            base.Initialize(manager, param);

            playerCondition = CoreManager.Instance.gameManager.Player.PlayerCondition;
            playerWeapon = CoreManager.Instance.gameManager.Player.PlayerWeapon;

            weaponPreviewMap = new();
            foreach (Transform child in previewSpawnPoint)
            {
                var handler = child.GetComponent<PreviewWeaponHandler>();
                if (handler)
                {
                    weaponPreviewMap[handler.weaponType] = child.gameObject;
                }
                child.gameObject.SetActive(false);
            }
            CacheAllPartData();

            applyButton.onClick.AddListener(OnApplyButtonClicked);
            confirmButton.onClick.AddListener(OnApplyConfirmed);
            cancelButton.onClick.AddListener(HideModal);
            prevButton.onClick.AddListener(OnPrevWeaponClicked);
            nextButton.onClick.AddListener(OnNextWeaponClicked);

            Hide();
            HideModal();
            ResetUI();
            Refresh();
        }
        public override void Show()
        {
            base.Show();
            playerCondition.OnDisablePlayerMovement();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Refresh();
        }
        public override void Hide()
        {
            Service.Log("Hide Modification UI");
            panel.SetActive(false);
            gameObject.SetActive(false);
            UnhighlightPart();
            playerCondition.OnEnablePlayerMovement();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        public override void ResetUI()
        {
            foreach (var go in weaponPreviewMap.Values)
                go.SetActive(false);
            previewWeaponHandler = null;
            selectedPartType = null;
            selectedPartData = null;
            UnhighlightPart();
            ResetStatUI();
            HideModal();
            currentWeaponIdx = 0;
            ResetPartButtons();
            
            if (weaponNameText) weaponNameText.text = "";
            if (partNameText) partNameText.text = "";
        }
        private void CacheAllPartData()
        {
            var allParts = CoreManager.Instance.resourceManager.GetAllAssetsOfType<WeaponPartData>();
            partDataMap = allParts.ToDictionary(x => (x.Type, x.Id), x => x);
        }
        private WeaponType GetSlotWeaponType(int slotIdx)
        {
            var role = SlotOrder[slotIdx];
            if (role == WeaponType.Pistol) return ownedWeapons.ContainsKey(WeaponType.SniperRifle) ? WeaponType.SniperRifle : WeaponType.Pistol; 
            return role;
        }

        private void Refresh()
        {
            SyncOwnedWeapons();
            if (!ownedWeapons.ContainsKey(GetSlotWeaponType(currentWeaponIdx)))
                SetCurrentWeapon();
            ShowWeaponPreview(CurrentWeaponType);
            selectedPartType = null;
            selectedPartData = null;
            UnhighlightPart();
            GeneratePartSlots();
            UpdateStatUI();
            UpdateNameUI();
        }
        private void SyncOwnedWeapons()
        {
            ownedWeapons.Clear();

            if (!playerWeapon) return;
            foreach (var (type, weapon) in playerWeapon.Weapons)
            {
                if (!weapon) continue;
                if (!playerWeapon.AvailableWeapons.TryGetValue(type, out var unlocked) || !unlocked) continue;
                if (type == WeaponType.Punch) continue;
                ownedWeapons[type] = weapon;
            }
        }
        
        private void SetCurrentWeapon()
        {
            for (int i = 0; i < SlotOrder.Length; i++)
            {
                if (!ownedWeapons.ContainsKey(SlotOrder[i])) continue;
                currentWeaponIdx = i;
                return;
            }
            currentWeaponIdx = 0;
        }
        
        private void ShowWeaponPreview(WeaponType weaponType)
        {
            foreach (var go in weaponPreviewMap.Values)
                go.SetActive(false);
            if (weaponPreviewMap.TryGetValue(weaponType, out var previewGo))
            {
                previewGo.SetActive(true);
                previewWeaponHandler = previewGo.GetComponent<PreviewWeaponHandler>();
            }
            else { previewWeaponHandler = null; }
            lastHighlightedPartType = null;
            lastSelectedPartMaterial = null;
        }
        
        private void GeneratePartSlots()
        {
            for (int i = 0; i < partButtons.Count; i++)
            {
                partButtons[i].gameObject.SetActive(false);
                if (partButtonTexts.Count > i && partButtonTexts[i]) partButtonTexts[i].text = "";
                if (partButtonImages.Count > i && partButtonImages[i]) partButtonImages[i].sprite = null;
                partButtons[i].onClick.RemoveAllListeners();
            }

            if (!CurrentWeapon)
            {
                applyButton.interactable = false;
                applyButton.gameObject.SetActive(true);
                SetLocalizedText(requiredText, "Require_SelectWeapon");
                return;
            }

            bool isPistol = CurrentWeaponType == WeaponType.Pistol;
            if (isPistol)
            {
                foreach (var btn in partButtons) btn.gameObject.SetActive(false);

                if (IsForgeAvailable(CurrentWeapon))
                {
                    applyButton.interactable = true;
                    SetLocalizedText(requiredText, "Require_ForgeAvailable");
                }
                else
                {
                    applyButton.interactable = false;
                    SetLocalizedText(requiredText, "Require_ForgeUnavailable");
                }
                return;
            }
            
            var partTypeList = GetPartTypes(CurrentWeaponType);
            bool anyPart = false;
            
            for (int i = 0; i < allPartTypes.Length; i++)
            {
                var partType = allPartTypes[i];
                
                if (partType == PartType.Sight && CurrentWeaponType != WeaponType.Rifle && i > 0)
                    continue;
                
                int partId = (CurrentWeaponType == WeaponType.Rifle && partType == PartType.Sight)
                    ? GetPartId(CurrentWeaponType, partType, i)
                    : GetPartId(CurrentWeaponType, partType);
                
                bool enabled = partTypeList.Contains(partType);
                bool hasPart = enabled && CurrentWeapon.EquipableWeaponParts.TryGetValue(partId, out var own) && own;
                bool isEquipped = false;
                
                if (CurrentWeapon is Gun { EquippedWeaponParts: not null } gun)
                    isEquipped = gun.EquippedWeaponParts.TryGetValue(partType, out int equippedId) && equippedId == partId;
                else if (CurrentWeapon.GetType().Name == "HackGun" && ((HackGun)CurrentWeapon).EquippedWeaponParts != null)
                    isEquipped = ((HackGun)CurrentWeapon).EquippedWeaponParts.TryGetValue(partType, out int equippedId) && equippedId == partId;
                else if (CurrentWeapon.GetType().Name == "GrenadeLauncher" && ((GrenadeLauncher)CurrentWeapon).EquippedWeaponParts != null)
                    isEquipped = ((GrenadeLauncher)CurrentWeapon).EquippedWeaponParts.TryGetValue(partType, out int equippedId) && equippedId == partId;

                partButtons[i].gameObject.SetActive(hasPart);

                if (hasPart)
                {
                    partButtons[i].interactable = !isEquipped;
                    if (partButtonImages.Count > i && partButtonImages[i])
                    {
                        partButtonImages[i].sprite = GetPartSprite(CurrentWeaponType, partType, partId);
                        partButtonImages[i].color = isEquipped
                            ? new Color(0.7f, 0.7f, 0.7f, 0.5f)
                            : Color.white;
                    }
                    if (partButtonTexts.Count > i && partButtonTexts[i])
                    {
                        if (partDataMap.TryGetValue((partType, partId), out var partData) && !string.IsNullOrEmpty(partData.NameKey))
                        {
                            var localized = new LocalizedString("New Table", partData.NameKey);
                            localized.StringChanged += val =>
                            {
                                partButtonTexts[i].text = val + (CurrentWeaponType == WeaponType.Rifle && partType == PartType.Sight ? $" ({partId})" : "");
                            };
                        }
                        else
                        {
                            partButtonTexts[i].text = partType.ToString() + (CurrentWeaponType == WeaponType.Rifle && partType == PartType.Sight ? $" ({partId})" : "");
                        }
                    }
                    partButtons[i].onClick.RemoveAllListeners();
                    if (!isEquipped)
                    {
                        partButtons[i].onClick.AddListener(() => OnPartButtonClicked(partType, partId));
                    }
                    anyPart = true;
                }
                else
                {
                    if (partButtonImages.Count > i && partButtonImages[i])
                        partButtonImages[i].sprite = null;
                    partButtons[i].onClick.RemoveAllListeners();
                }
            }

            applyButton.gameObject.SetActive(true);

            if (!anyPart)
            {
                applyButton.interactable = false;
                SetLocalizedText(requiredText, "Require_ModificationUnavailable");
            }
            else
            {
                applyButton.interactable = false;
                SetLocalizedText(requiredText, "Require_SelectAvailablePart");
            }
        }
        private void ResetPartButtons()
        {
            for (int i = 0; i < partButtons.Count; i++)
            {
                partButtons[i].gameObject.SetActive(false);
                if (partButtonTexts.Count > i && partButtonTexts[i])
                    partButtonTexts[i].text = "";
                if (partButtonImages.Count > i && partButtonImages[i])
                    partButtonImages[i].sprite = null;
                partButtons[i].onClick.RemoveAllListeners();
            }
        }
        
        private List<PartType> GetPartTypes(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Rifle:
                    return new List<PartType>{ PartType.FlameArrester,  PartType.Silencer, PartType.Suppressor, PartType.Sight };
                case WeaponType.GrenadeLauncher:
                    return new List<PartType>{ PartType.Sight };
                case WeaponType.HackGun:
                    return new List<PartType>{ PartType.ExtendedMag };
                case WeaponType.Pistol:
                case WeaponType.Punch:
                case WeaponType.SniperRifle:
                default:
                    return new List<PartType>();
            }
        }
        private void OnPartButtonClicked(PartType partType, int partId)
        {
            selectedPartType = partType;
            selectedPartId = partId;
            
            if (!partDataMap.TryGetValue((partType, partId), out selectedPartData))
            {
                SetLocalizedText(requiredText, "Require_NoPartDataFound");
                applyButton.interactable = false;
                return;
            }

            HighlightPart(partType);

            UpdateStatPreview(selectedPartData);

            bool hasPart = CurrentWeapon.EquipableWeaponParts.TryGetValue(partId, out var own) && own;
            SetLocalizedText(requiredText, hasPart ? "Require_Available" : "Require_Unavailable");
            applyButton.interactable = hasPart;
            applyButton.gameObject.SetActive(true);

            UpdateNameUI();

            applyButton.onClick.RemoveAllListeners();
            applyButton.onClick.AddListener(OnApplyButtonClicked);
        }
        private void UpdateStatPreview(WeaponPartData partData)
        {
            var stat = SlotUtility.GetWeaponStat(CurrentWeapon);
            
            float newRecoil = stat.Recoil + partData.ReduceRecoilRate * stat.Recoil;
            int newAmmo = stat.MaxAmmoCountInMagazine + partData.IncreaseMaxAmmoCountInMagazine;
            
            recoilText.text = $"{newRecoil}";
            ammoText.text = $"{newAmmo}";

            int maxDamage = 1000;
            float maxRPM = 100f;
            float maxRecoil = 100f;
            float maxWeight = 10f;
            int maxAmmo = 60;
            
            recoilSlider.value = newRecoil / maxRecoil;
            ammoSlider.value = (float)newAmmo / maxAmmo;
        }

        private Sprite GetPartSprite(WeaponType weaponType, PartType partType, int partId)
        {
            if (partDataMap != null && partDataMap.TryGetValue((partType, partId), out var partData))
                return partData.Icon;
            return null;
        }
        
        private int GetPartId(WeaponType weaponType, PartType partType, int buttonIdx = 0)
        {
            switch (weaponType)
            {
                case WeaponType.Rifle when partType == PartType.Sight:
                    return (buttonIdx == 0) ? 1 : 2;
                case WeaponType.Rifle:
                    switch (partType)
                    {
                        case PartType.ExtendedMag: return 11;
                        case PartType.FlameArrester: return 6;
                        case PartType.Sight: return 1;
                        case PartType.Silencer: return 7;
                        case PartType.Suppressor: return 10;
                    }

                    break;
                case WeaponType.GrenadeLauncher when partType == PartType.Sight:
                    return 5;
                case WeaponType.HackGun:
                    switch (partType)
                    {
                        case PartType.ExtendedMag: return 12;
                        case PartType.Sight: return 4;
                        case PartType.FlameArrester: return 8;
                    }

                    break;
                case WeaponType.Pistol:
                    switch (partType)
                    {
                        case PartType.Sight: return 14;
                        case PartType.Silencer: return 15;
                        case PartType.ExtendedMag: return 13;
                    }
                    break;
                default: return -1;
            }
            return -1;
        }

        
        private void HighlightPart(PartType partType)
        {
            UnhighlightPart();
            if (!previewWeaponHandler) return;
            var renderer = previewWeaponHandler.GetRendererOfPart(partType);
            if (!renderer) return;
            lastHighlightedPartType = partType;
            lastSelectedPartMaterial = renderer.material;
            renderer.material = partHighlightMaterial;
        }

        private void UnhighlightPart()
        {
            if (!previewWeaponHandler || lastHighlightedPartType == null) return;
            var renderer = previewWeaponHandler.GetRendererOfPart(lastHighlightedPartType.Value);
            if (renderer && lastSelectedPartMaterial) renderer.material = lastSelectedPartMaterial;
            lastHighlightedPartType = null;
            lastSelectedPartMaterial = null;
        }

        private void OnApplyButtonClicked()
        {
            if (!applyButton.interactable) return;
            ShowModal();
        }

        private void OnApplyConfirmed()
        {
            HideModal();
            bool applied = false;
            if (CurrentWeaponType == WeaponType.Pistol && IsForgeAvailable(CurrentWeapon))
            {
                applied = playerWeapon.ForgeWeapon();
            }
            else if (selectedPartType != null && selectedPartData)
            {
                applied = playerWeapon.EquipPart(CurrentWeaponType, selectedPartType.Value, selectedPartId);
            }
            if (applied) Refresh();
            else Debug.LogError("Failed to apply part");
        }
        
        private bool IsForgeAvailable(BaseWeapon weapon)
        {
            if (weapon is Gun gun && gun.GunData.GunStat.Type == WeaponType.Pistol)
            {
                int sightId = GetPartId(WeaponType.Pistol, PartType.Sight);
                int extId = GetPartId(WeaponType.Pistol, PartType.ExtendedMag);
                int silencerId = GetPartId(WeaponType.Pistol, PartType.Silencer);

                return gun.EquipableWeaponParts.TryGetValue(sightId, out var hasSight) && hasSight
                    && gun.EquipableWeaponParts.TryGetValue(extId, out var hasExt) && hasExt
                    && gun.EquipableWeaponParts.TryGetValue(silencerId, out var hasSilencer) && hasSilencer;
            }
            return false;
        }

        private void ShowModal()
        {
            applyModal.SetActive(true);
            modalWindowManager.ModalWindowIn();
        }

        private void HideModal()
        {
            if (!applyModal.activeInHierarchy) return;
            modalWindowManager.ModalWindowOut();
            applyModal.SetActive(false);
        }

        private void UpdateStatUI()
        {
            if (!CurrentWeapon)
            {
                ResetStatUI();
                return;
            }
            var stat = SlotUtility.GetWeaponStat(CurrentWeapon);
            int maxDamage = 1000;
            float maxRPM = 100f;
            float maxRecoil = 100f;
            float maxWeight = 10f;
            int maxAmmo = 60;

            damageSlider.value = (float)stat.Damage / maxDamage;
            rpmSlider.value = stat.Rpm / maxRPM;
            recoilSlider.value = stat.Recoil / maxRecoil;
            weightSlider.value = stat.Weight / maxWeight;
            ammoSlider.value = (float)stat.MaxAmmoCountInMagazine / maxAmmo;

            damageText.text = stat.Damage.ToString();
            rpmText.text = Mathf.RoundToInt(stat.Rpm).ToString();
            recoilText.text = Mathf.RoundToInt(stat.Recoil).ToString();
            weightText.text = stat.Weight.ToString("F1");
            ammoText.text = stat.MaxAmmoCountInMagazine.ToString();
        }
        private void ResetStatUI()
        {
            damageText.text = rpmText.text = recoilText.text = weightText.text = ammoText.text = "";
            damageSlider.value = rpmSlider.value = recoilSlider.value = weightSlider.value = ammoSlider.value = 0f;
        }
        
        private void UpdateNameUI()
        {
            if (weaponNameText)
            {
                if (CurrentWeapon)
                    SlotUtility.GetWeaponName(CurrentWeapon, weaponNameText);
                else
                    weaponNameText.text = "";
            }
            if (partNameText)
            {
                if (selectedPartData && !string.IsNullOrEmpty(selectedPartData.NameKey))
                {
                    var localized = new LocalizedString("New Table", selectedPartData.NameKey);
                    localized.StringChanged += val => partNameText.text = val;
                }
                else
                {
                    partNameText.text = selectedPartType?.ToString() ?? "";
                }
            }
            if (partDescriptionText)
            {
                if (selectedPartData && !string.IsNullOrEmpty(selectedPartData.DescKey))
                {
                    var localizedDesc = new LocalizedString("New Table", selectedPartData.DescKey);
                    localizedDesc.StringChanged += val => partDescriptionText.text = val;
                }
                else
                {
                    partDescriptionText.text = "";
                }
            }
        }
        private void SetLocalizedText(TextMeshProUGUI text, string key)
        {
            if (!text || string.IsNullOrEmpty(key)) return;
            var localized = new LocalizedString("New Table", key);
            localized.StringChanged += val => text.text = val;
        }
        private void OnPrevWeaponClicked()
        {
            for (int i = 1; i <= SlotOrder.Length; i++)
            {
                int idx = (currentWeaponIdx - i + SlotOrder.Length) % SlotOrder.Length;
                if (!ownedWeapons.ContainsKey(GetSlotWeaponType(idx))) continue;
                currentWeaponIdx = idx;
                break;
            }
            Refresh();
        }

        private void OnNextWeaponClicked()
        {
            for (int i = 1; i <= SlotOrder.Length; i++)
            {
                int idx = (currentWeaponIdx + i) % SlotOrder.Length;
                if (!ownedWeapons.ContainsKey(GetSlotWeaponType(idx))) continue;
                currentWeaponIdx = idx;
                break;
            }
            Refresh();
        }
    }
}