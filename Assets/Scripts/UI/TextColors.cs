using UnityEngine;
using Color = UnityEngine.Color;

public static class TextColors
{
    static Color m_NormalTextColor = new Color32(255, 255, 255, 255);
    static Color m_MagicDamageTextColor = new Color32(61, 162, 186, 255);
    static Color m_LifeTextColor = new Color32(82, 186, 23, 255);
    static Color m_AdditionalLifeTextColor = new Color32(106, 168, 79, 255);
    static Color m_AttackDamageTextColor = new Color32(230, 145, 56, 255);
    static Color m_FisicDamageTextColor = new Color32(224, 102, 102, 255);
    static Color m_SkillPowerTextColor = new Color32(154, 143, 183, 255);
    public enum TextColorTypes
    {
        NORMAL,
        MAGICDAMAGE,
        LIFE,
        ADDITIONALLIFE,
        ATTACKDAMAGE,
        FISICDAMAGE,
        SKILLPOWER
    }
    public static string ColorTheText(string TextToColor)
    {
        string l_ColoredText = TextToColor;


        l_ColoredText = l_ColoredText.Replace("NORMAL", GetColorHEX(TextColorTypes.NORMAL));
        l_ColoredText = l_ColoredText.Replace("MAGICDAMAGE", GetColorHEX(TextColorTypes.MAGICDAMAGE));
        l_ColoredText = l_ColoredText.Replace("LIFE", GetColorHEX(TextColorTypes.LIFE));
        l_ColoredText = l_ColoredText.Replace("ADDITIONALLIFE", GetColorHEX(TextColorTypes.ADDITIONALLIFE));
        l_ColoredText = l_ColoredText.Replace("ATTACKDAMAGE", GetColorHEX(TextColorTypes.ATTACKDAMAGE));
        l_ColoredText = l_ColoredText.Replace("FISICDAMAGE", GetColorHEX(TextColorTypes.FISICDAMAGE));
        l_ColoredText = l_ColoredText.Replace("SKILLPOWER", GetColorHEX(TextColorTypes.SKILLPOWER));

        return l_ColoredText;
    }

    public static string GetColorHEX(TextColorTypes ColorType)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(GetColor(ColorType));
    }
    public static Color GetColor(TextColorTypes ColorType)
    {
        Color l_Color = Color.white;

        switch (ColorType)
        {
            case TextColorTypes.NORMAL:
                l_Color = m_NormalTextColor;
                break;
            case TextColorTypes.MAGICDAMAGE:
                l_Color = m_MagicDamageTextColor;
                break;
            case TextColorTypes.LIFE:
                l_Color = m_LifeTextColor;
                break;
            case TextColorTypes.ADDITIONALLIFE:
                l_Color = m_AdditionalLifeTextColor;
                break;
            case TextColorTypes.ATTACKDAMAGE:
                l_Color = m_AttackDamageTextColor;
                break;
            case TextColorTypes.FISICDAMAGE:
                l_Color = m_FisicDamageTextColor;
                break;
            case TextColorTypes.SKILLPOWER:
                l_Color = m_SkillPowerTextColor;
                break;

        }
        return l_Color;
    }
}