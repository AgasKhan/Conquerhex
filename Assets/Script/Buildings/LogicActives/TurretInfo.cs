using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretInfo : LogicActive<Building>
{
    public override void Activate(Building specificParam)
    {
        var aux = specificParam;

        if (aux.currentLevel == 0)
        {
            //aux.myBuildSubMenu.detailsWindow.SetTexts("", aux.flyweight.GetDetails()["Description"]);
            //aux.myBuildSubMenu.detailsWindow.SetImage(aux.flyweight.image);
        } 
        else
        {
            string newText = "\nHabilidades: \n".RichText("color", "#00ffffff"); ;

            foreach (var item in aux.flyweight.GetFlyWeight<AttackBase>().kataCombos)
            {
                //Filtrar cual tiene equipada el jugador 

                newText += item.kata.nameDisplay.RichText("color", "#ffa500ff") +"\n";
                newText += item.kata.GetDetails().ToString("\n");
            }

            if(aux.flyweight.GetFlyWeight<AttackBase>().additiveDamage.Length > 0)
            {
                newText += "\nAdditive Damage: ".RichText("color", "#00ffffff");
                //newText += aux.flyweight.additiveDamage[0].typeInstance.ToString() + " x " + aux.flyweight.additiveDamage[0].ToString();
                newText += aux.flyweight.GetDetails()["Description"] + " x " + aux.flyweight.GetFlyWeight<AttackBase>().additiveDamage[0].ToString();
            } 

            //aux.myBuildSubMenu.detailsWindow.SetTexts("Turret " + ((TurretBuild)aux).originalAbility, newText);
            //aux.myBuildSubMenu.detailsWindow.SetImage(((TurretBuild)aux).myStructure.possibleAbilities[((TurretBuild)aux).originalAbility][aux.currentLevel - 1]);

        }

        //aux.myBuildSubMenu.DestroyCraftButtons();
    }
}
