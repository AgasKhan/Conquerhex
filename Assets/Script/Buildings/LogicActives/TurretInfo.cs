using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretInfo : LogicActive<Building>
{
    protected override void InternalActivate(params Building[] specificParam)
    {
        var aux = specificParam[0];

        if (aux.currentLevel == 0)
        {
            aux.myBuildSubMenu.detailsWindow.SetTexts("", aux.flyweight.GetDetails()["Description"]);
            aux.myBuildSubMenu.detailsWindow.SetImage(((TurretBuild)aux).baseSprite);
        } 
        else
        {
            string newText = "\nHabilidades: \n".RichText("color", "#00ffffff"); ;

            foreach (var item in aux.flyweight.kataCombos)
            {
                //Filtrar cual tiene equipada el jugador 

                newText += item.kata.nameDisplay.RichText("color", "#ffa500ff") +"\n";
                newText += item.kata.GetDetails().ToString();
            }

            if(aux.flyweight.additiveDamage.Length > 0)
            {
                newText += "\nAdditive Damage: ".RichText("color", "#00ffffff");
                //newText += aux.flyweight.additiveDamage[0].typeInstance.ToString() + " x " + aux.flyweight.additiveDamage[0].ToString();
                newText += aux.flyweight.GetDetails()["Description"].ToString() + " x " + aux.flyweight.additiveDamage[0].ToString();
            } 

            aux.myBuildSubMenu.detailsWindow.SetTexts("Turret " + ((TurretBuild)aux).originalAbility, newText);
            aux.myBuildSubMenu.detailsWindow.SetImage(((TurretBuild)aux).possibleAbilities[((TurretBuild)aux).originalAbility][aux.currentLevel - 1]);

        }

        aux.myBuildSubMenu.DestroyCraftButtons();
    }
}
