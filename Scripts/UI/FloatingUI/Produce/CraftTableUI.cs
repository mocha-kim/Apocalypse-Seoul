using ItemSystem.Produce;
using Manager;
using TMPro;
using UnityEngine;

namespace UI.FloatingUI.Produce
{
    public class CraftTableUI : ProducerUI
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        
        protected override ProducerType GetProducerType() => ProducerType.CraftingTable;
        public override UIType GetUIType() => UIType.CraftTableUI;

        public override void Open()
        {
            base.Open();

            // Todo: load from producer csv, language csv
            _nameText.text = "재작대 " + DataManager.Stat.ProducerLevel[GetProducerType()] + " 레벨";
        }
    }
}