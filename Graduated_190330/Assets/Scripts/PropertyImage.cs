using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Text propertyName;
    [SerializeField] Text propertyDescription;
    public Toggle PropertyToggle;
    public CharacterProperty Property;
    public E_PropertyType propertyType;

    void Start()
    {
        PropertyToggle.onValueChanged.AddListener((isOn) => { OnChangedToggle(); });
    }

    public void SetProperty(CharacterProperty property)
    {
        if (property == null)
            return;

        Property = property;
        SetPropertyImage();
    }

    void SetPropertyImage()
    {
        if (propertyName == null)
            return;

        if (propertyDescription == null)
            return;

        propertyName.text = Property.Name;
        propertyDescription.text = Property.Description;
        image.sprite = Resources.Load<Sprite>(Property.ImagePath);
    }

    void OnChangedToggle()
    {
        if (PropertyToggle.isOn)
        {
            CharacterPropertyManager.Instance.OnChangedProperty(propertyType);
        }
    }


}
