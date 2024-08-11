using DataSystem.Database;
using ItemSystem.Produce;
using UI;
using UnityEngine;

namespace InteractableObject
{
    public class FieldProducerObject : ProducerObject
    {
        [SerializeField] private int _id;
        protected override ProducerType GetProducerType() => ProducerType.Field;

        protected override UIType GetUIType() => UIType.FieldProducerUI;

        protected override void Start()
        {
            data = Database.GetProducer(_id);
            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            base.Start();
        }
    }
}