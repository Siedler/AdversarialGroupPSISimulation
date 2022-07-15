using Scrips.UI;
using TMPro;
using UnityEngine;

public class UINeedTankController : MonoBehaviour, UINeedTankInterface {

    public string needName;
    
    private TMP_InputField _isValueText;
    private TMP_InputField _setValueText;
    private TMP_InputField _leakageText;

    // Start is called before the first frame update
    void Start() {
        transform.Find("Need Label").GetComponent<TMP_Text>().text = needName;
        
        _isValueText = transform.Find("Is Value/Is Value Text").GetComponent<TMP_InputField>();
        _setValueText = transform.Find("Set Value/Set Value Text").GetComponent<TMP_InputField>();
        _leakageText = transform.Find("Leakage/Leakage Text").GetComponent<TMP_InputField>();
    }

    public void SetValues(double isValue, double setValue, double leakageValue) {
        SetIsValue(isValue);
        SetSetValue(setValue);
        SetLeakage(leakageValue);
    }

    public void SetIsValue(double isValue) {
        _isValueText.text = isValue.ToString();
    }

    public void SetSetValue(double setValue) {
        _setValueText.text = setValue.ToString();
    }

    public void SetLeakage(double leakageValue) {
        _leakageText.text = leakageValue.ToString();
    }
}
