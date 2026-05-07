using UnityEngine ;

namespace EasyUI.PickerWheelUI {
   [System.Serializable]
   public class WheelPiece {
      public UnityEngine.Sprite Icon ;
      public string Label ;
      public bool IsPositive = false;
      public bool IsRandomizedChance = false;
      public EffectBehaviour CustomFX;

      [Tooltip ("Reward amount")] public int Amount ;

      public Color32 LabelColor;
      public Color32 AmountColor;

      [Tooltip ("Probability in %")] 
      [Range (0f, 100f)] 
      public float Chance = 100f ;

      [HideInInspector] public int Index ;
      [HideInInspector] public double _weight = 0f ;
   }
}
