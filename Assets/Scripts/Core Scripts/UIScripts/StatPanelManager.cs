using System.Globalization;
using TMPro;
using UnityEngine;

public class StatPanelManager : MonoBehaviour
{
    public TMP_Text mightText;
    public TMP_Text witText;
    public TMP_Text willText;
    public TMP_Text intelligenceText;
    public TMP_Text healthText;
    public TMP_Text speedText;
    public TMP_Text physDamagePointsText;
    public TMP_Text magicalDamagePointsText;
    public TMP_Text physicalArmorText;
    public TMP_Text magicArmor;
    public TMP_Text physicalDamagePercent;
    public TMP_Text magicalDamagePercent;
    public TMP_Text physicalResistance;
    public TMP_Text magicResistance;
    public TMP_Text gestation;
    public TMP_Text tenacity;
    public TMP_Text armorRating;
    public TMP_Text attackRating;
    public TMP_Text attackDamageRange;


    public void UpdateStatPanelText(PlayerStatBlock statBlock)
    {
        mightText.SetText(statBlock.GetMight().ToString());
        witText.SetText(statBlock.GetWit().ToString());
        willText.SetText(statBlock.GetWill().ToString(CultureInfo.CurrentCulture));
        intelligenceText.SetText(statBlock.GetIntelligence().ToString());
        healthText.SetText(statBlock.currentHealth.Value + " / " + statBlock.maxHealth.Value);
        speedText.SetText(statBlock.GetSpeedStat().ToString(CultureInfo.CurrentCulture));
        physDamagePointsText.SetText(statBlock.GetPhysicalDamagePoints().ToString());
        magicalDamagePointsText.SetText(statBlock.GetMagicalDamagePoints().ToString());
        physicalArmorText.SetText(statBlock.GetPhysicalArmor().ToString());
        magicArmor.SetText(statBlock.GetMagicArmor().ToString());
        physicalDamagePercent.SetText(statBlock.GetPhysicalDamagePercent().ToString(CultureInfo.CurrentCulture));
        magicalDamagePercent.SetText(statBlock.GetMagicDamagePercent().ToString(CultureInfo.InvariantCulture));
        physicalResistance.SetText(statBlock.GetPhysicalResistance().ToString(CultureInfo.CurrentCulture));
        magicResistance.SetText(statBlock.GetMagicResistance().ToString(CultureInfo.CurrentCulture));
        gestation.SetText(statBlock.GetGestation().ToString(CultureInfo.InvariantCulture));
        tenacity.SetText(statBlock.GetTenacity().ToString(CultureInfo.CurrentCulture));
        armorRating.SetText(statBlock.GetArmorRating().ToString());
        attackRating.SetText(statBlock.GetAttackRating().ToString());
        attackDamageRange.SetText(statBlock.GetAttackRangeText());
    }
}