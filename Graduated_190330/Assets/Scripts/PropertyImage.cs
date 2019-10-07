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
    public E_PropertyEffectType propertyEffectType;

    void Start()
    {
        if(PropertyToggle != null)
            PropertyToggle.onValueChanged.AddListener((isOn)=>{OnChangedToggle();});
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
        if (propertyDescription == null)
            return;

        if(propertyName != null)
            propertyName.text = Property.Name;

        propertyDescription.text = Property.Description;
        image.sprite = Resources.Load<Sprite>(Property.ImagePath);
    }

    public void OnChangedToggle()
    {
        if (PropertyToggle.isOn)
        {
            CharacterPropertyManager.Instance.OnChangedBattleProperty(propertyEffectType);
        }
    }
}
