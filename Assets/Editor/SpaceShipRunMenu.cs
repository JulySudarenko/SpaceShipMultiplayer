using UnityEditor;

namespace Editor
{
    public class SpaceShipRunMenu
    {
        [MenuItem("SpaceShipRun/PlanetSystemMaker")]
        private static void MenuPlanetCreator()
        {
            EditorWindow.GetWindow(typeof(PlanetsSystemCreatorWindow), false, "PlanetSystemMaker");
        }
    }
}
