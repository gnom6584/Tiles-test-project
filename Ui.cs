using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Ui : MonoBehaviour
{
    public UnityEvent<App.Parameters> ParametersChanged;

    [SerializeField] InputField _spacingField;

    [SerializeField] InputField _rotationField;

    [SerializeField] InputField _offsetField;

    [SerializeField] Text _areaTextField;

    bool _parametersChanged = true;

    public void SetTileArea(float area) => _areaTextField.text = area.ToString("0.000");

    void Awake()
    {
        _spacingField.characterValidation = InputField.CharacterValidation.Decimal;    
        _rotationField.characterValidation = InputField.CharacterValidation.Decimal;    
        _offsetField.characterValidation = InputField.CharacterValidation.Decimal;
        _spacingField.onValueChanged.AddListener((_) => _parametersChanged = true);
        _rotationField.onValueChanged.AddListener((_) => _parametersChanged = true);
        _offsetField.onValueChanged.AddListener((_) => _parametersChanged = true);
    }

    void Update()
    {
        if(_parametersChanged)
        {
            try
            {
                ParametersChanged?.Invoke(new App.Parameters(float.Parse(_spacingField.text) * 0.001f, float.Parse(_rotationField.text), float.Parse(_offsetField.text) * 0.001f));
            }
            catch(System.Exception _)
            {

            }
            _parametersChanged = false;
        }
    }
}

