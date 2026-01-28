using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatInteract : InteractableObject
{
    [SerializeField] private MicrobeSO _microbe;
    [SerializeField] private BoolGameEventSO _gotMicrobeSO;
    [SerializeField] private StringGameEventSO _menuToggleStringSO;
    private AddMicrobeToggler _menuToggler;

    void Start()
    {
        _menuToggler = gameObject.AddComponent(typeof(AddMicrobeToggler)) as AddMicrobeToggler;
        _menuToggler.SetMicrobeSO(_microbe);
        SetInteractText(_microbe.microbeName);
    }

    public override void Interact()
    {
        if (_gotMicrobeSO != null)
        {
            _gotMicrobeSO.Raise(false);
        }

        if (_menuToggleStringSO != null)
        {
            _menuToggleStringSO.Raise("AddMicrobeToPlayerMenu");
        }
        
        if(_menuToggler != null)
        {
            _menuToggler.PopulateMenuData();
        }
    }
}
