using EasyUI.PickerWheelUI;
using UnityEngine;

public class TeamPickerControl : MonoBehaviour
{
    [SerializeField] private PickerWheel pickerWheel;

    public EffectBehaviour OnSpinFX;
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
        if(currentwheelpiece.CustomFX != null)
            currentwheelpiece.CustomFX.PlayFX();

        foreach (WheelPiece wheelpiece in pickerWheel.wheelPieces)
        {
            if (wheelpiece.IsRandomizedChance)
                wheelpiece.Chance = Random.Range(0, 100f);
        }
    }
}
