using UnityEngine;

 [System.Serializable] //Sagt Unity, dass diese Klasse im Inspektor angezeigt werden soll
public class DialogueChoice //kein monobehaviour!
{
    public string text; //Text der Wahl
    public DialogueLine nextLine; //Nächste Zeile, die angezeigt wird, wenn diese Wahl getroffen wird
    public string id; 

}
