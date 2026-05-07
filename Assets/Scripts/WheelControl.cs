using EasyUI.PickerWheelUI;
using UnityEngine;


public class WheelControl : MonoBehaviour
{
    [SerializeField]private PickerWheel pickerWheel;

    public EffectBehaviour OnSpinFX;
    public EffectBehaviour OnPositiveFX;
    public EffectBehaviour OnNegativeFX;
    public void StartSpinning()
    {
        pickerWheel.Spin();
        OnSpinFX.PlayFX();

        pickerWheel.OnSpinEnd(wheelPiece => {
            SpinEndAction(wheelPiece);
        });
    }
    
    void SpinEndAction(WheelPiece currentwheelpiece)
    {
        if (currentwheelpiece.IsPositive)
            OnPositiveFX.PlayFX();
        else
            OnNegativeFX.PlayFX();

        foreach(WheelPiece wheelpiece in pickerWheel.wheelPieces)
        {
            if(wheelpiece.IsRandomizedChance)
               wheelpiece.Chance = Random.Range(0, 100f);
        }
    }
}
