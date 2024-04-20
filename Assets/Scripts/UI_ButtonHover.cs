using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UI_ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI ButtonText;
    public Color HoverColor;
    public Image ButtonIcon;
    public Sprite HoverSprite;
    public Image ButtonBG;

    private Color IdleColor;
    private Sprite IdleSprite;

    void Start()
    {
        // Store the original color of the text
        IdleColor = ButtonText.color;
        IdleSprite = ButtonIcon.sprite;
    }

    // Called when the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change the text color to the hover color
        ButtonText.color = HoverColor;

        // Toggle the underlay effect on
        ButtonText.fontMaterial.EnableKeyword("UNDERLAY_ON");
        ButtonText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 1f);
        ButtonText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -1f);
        ButtonText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 1f);

        // Change the Icon sprite to the hover sprite
        ButtonIcon.sprite = HoverSprite;

        // Change the ButtonBG to visible
        ButtonBG.enabled = true;
    }

    // Called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        // Restore the original text color
        ButtonText.color = IdleColor;

        // Toggle the underlay effect off
        ButtonText.fontMaterial.DisableKeyword("UNDERLAY_ON");

        // Restore the original Icon Sprite
        ButtonIcon.sprite = IdleSprite;

        // Change the ButtonBG to invisible
        ButtonBG.enabled = false;
    }
}