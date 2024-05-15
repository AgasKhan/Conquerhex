||| Simple Cheat Prompt |||

>> ¿Qué es?
Te permite abrir una consola, mientras corres el juego, para ejecutar métodos que creaste (comandos) con el fin de facilitar el testeo del juego.

/*////////////////////////////////////////////////////////////////////////////////////////////*/

>> ¿Cómo funciona?
1. Dale play al juego.
2. Apreta "F1".

Verás como se abre la consola en la parte superior de la ventana, donde podrás escribir el nombre de tus comandos. Por ahora no funcionará, porque no creaste ningún comando.

/*////////////////////////////////////////////////////////////////////////////////////////////*/

>> ¿Cómo creo un comando?
Ve a cualquier clase (te recomiendo que sea en el GameManager o una clase sola para los comandos), y crea un método público y estático. Ahora agrégale el atributo [Command].
Ej.:
	public class GameManager
	{
		[Command]
		public static void Reset()
		{
			// ...
			// Reset the level.
			// ...
		}
	}

Listo! ahora el prompt reconocerá el comando y lo agregará a la lista.
Puedes crear comandos con los parámetros que quieras, menos con parámetros con valores por defecto.

/*////////////////////////////////////////////////////////////////////////////////////////////*/

>> Parámetros para el atributo
Puedes agregar un friendly name y una descripción al atributo. Los valores están por defecto como null, asi que puedes agregar uno, otro o los dos.
Ej. con el método anterior:

	[Command(name:"Reset", description:"Reset the level")]
	public static void ResetLevel()
	{
		// ...
		// Reset the level.
		// ...
	}

Si no defines el friendly name entonces se usará el nombre del método.