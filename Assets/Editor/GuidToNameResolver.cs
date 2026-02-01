using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class PrefabLibraryPopulator : EditorWindow
{
    // Reference to the ScriptableObject we want to fill
    PrefabLibrary targetLibrary; 
    
    string inputJson = "[\"GUID_HERE\", \"GUID_HERE\"]";
    Vector2 scrollPos;

    [MenuItem("Tools/Fill Prefab Library")]
    public static void ShowWindow()
    {
        GetWindow<PrefabLibraryPopulator>("Library Populator");
    }


public static readonly Dictionary<string, string> NameMapping = new Dictionary<string, string>
    {
        { "apple", "Apple" },
        { "apple_green", "Green Apple" },
        { "aubergine", "Aubergine" },
        { "avocado_full", "Avocado (Full)" },
        { "avocado_half", "Avocado (Half)" },
        { "bacon_tray", "Bacon Tray" },
        { "banana", "Banana" },
        { "bread1", "Bread (Type 1)" },
        { "bread2", "Bread (Type 2)" },
        { "burgers_tray", "Burgers Tray" },
        { "carrot", "Carrot" },
        { "cheese", "Cheese" },
        { "chicken", "Chicken" },
        { "couliflower", "Cauliflower" }, // Fixed typo
        { "croissant", "Croissant" },
        { "cucumber", "Cucumber" },
        { "donut_blue", "Blue Donut" },
        { "donut_orange", "Orange Donut" },
        { "donut_pink", "Pink Donut" },
        { "donut_red", "Red Donut" },
        { "donut_white", "White Donut" },
        { "egg_single", "Single Egg" },
        { "hotdog", "Hot Dog" },
        { "kiwi", "Kiwi" },
        { "leek", "Leek" },
        { "lemon", "Lemon" },
        { "lettuce", "Lettuce" },
        { "meat", "Meat" },
        { "mushroom_brown", "Brown Mushroom" },
        { "mushroom_orange", "Orange Mushroom" },
        { "mushroom_white", "White Mushroom" },
        { "onion", "Onion" },
        { "orange", "Orange" },
        { "peach", "Peach" },
        { "pear", "Pear" },
        { "pineapple", "Pineapple" },
        { "potato", "Potato" },
        { "pumpkin", "Pumpkin" },
        { "raspberry", "Raspberry" },
        { "sausages_tray", "Sausages Tray" },
        { "steak", "Steak" },
        { "strawberry", "Strawberry" },
        { "tomato", "Tomato" },
        { "watermelon_half", "Watermelon (Half)" },
        { "watermelon_quart", "Watermelon (Quart)" },
        { "watermelon_slice", "Watermelon (Slice)" },
        { "watermelon_whole", "Watermelon (Whole)" },
        { "bottle1_cola", "Bottle Cola (Type 1)" },
        { "bottle1_lime", "Bottle Lime (Type 1)" },
        { "bottle1_orange", "Bottle Orange (Type 1)" },
        { "bottle1_water", "Bottle Water (Type 1)" },
        { "bottle2_green", "Bottle Green (Type 2)" },
        { "bottle2_orange", "Bottle Orange (Type 2)" },
        { "bottle2_pink", "Bottle Pink (Type 2)" },
        { "bottle2_red", "Bottle Red (Type 2)" },
        { "bottle2_water", "Bottle Water (Type 2)" },
        { "bottle3_cola", "Bottle Cola (Type 3)" },
        { "bottle3_lime", "Bottle Lime (Type 3)" },
        { "bottle3_orange", "Bottle Orange (Type 3)" },
        { "bottle3_water", "Bottle Water (Type 3)" },
        { "bottle_big_green", "Large Green Bottle" },
        { "bottle_big_orange", "Large Orange Bottle" },
        { "bottle_big_pink", "Large Pink Bottle" },
        { "bottle_big_red", "Large Red Bottle" },
        { "bottle_big_water", "Large Water Bottle" },
        { "bottle_flat_blue", "Flat Blue Bottle" },
        { "bottle_flat_cream", "Flat Cream Bottle" },
        { "bottle_flat_darkBlue", "Flat Dark Blue Bottle" },
        { "bottle_flat_darkRed", "Flat Dark Red Bottle" },
        { "bottle_flat_green", "Flat Green Bottle" },
        { "bottle_flat_orange", "Flat Orange Bottle" },
        { "bottle_flat_pink", "Flat Pink Bottle" },
        { "bottle_flat_red", "Flat Red Bottle" },
        { "bottle_glass_green", "Glass Green Bottle" },
        { "bottle_glass_red", "Glass Red Bottle" },
        { "bottle_small_green", "Small Green Bottle" },
        { "bottle_small_orange", "Small Orange Bottle" },
        { "bottle_small_pink", "Small Pink Bottle" },
        { "bottle_small_red", "Small Red Bottle" },
        { "bottle_small_water", "Small Water Bottle" },
        { "bottle_yogurt_banana", "Banana Yogurt" },
        { "bottle_yogurt_blueberry", "Blueberry Yogurt" },
        { "bottle_yogurt_peach", "Peach Yogurt" },
        { "bottle_yogurt_strawberry", "Strawberry Yogurt" },
        { "box_cereal_granola", "Cereal (Granola)" },
        { "box_cereal_rings", "Cereal (Rings)" },
        { "box_cookies", "Cookies" },
        { "box_cookies_christmas", "Christmas Cookies" },
        { "box_cookies_valentine", "Valentine Cookies" },
        { "can_blue", "Blue Can" },
        { "can_brown", "Brown Can" },
        { "can_chips_bbq", "BBQ Chips" },
        { "can_chips_cheddar", "Cheddar Chips" },
        { "can_chips_classic", "Classic Chips" },
        { "can_chips_onion", "Onion Chips" },
        { "can_food_blue", "Blue Food Can" },
        { "can_food_brown", "Brown Food Can" },
        { "can_food_orange", "Orange Food Can" },
        { "can_food_red", "Red Food Can" },
        { "can_food_small_blue", "Small Blue Food Can" },
        { "can_food_small_brown", "Small Brown Food Can" },
        { "can_food_small_orange", "Small Orange Food Can" },
        { "can_food_small_red", "Small Red Food Can" },
        { "can_food_square_blue", "Square Blue Food Can" },
        { "can_food_square_brown", "Square Brown Food Can" },
        { "can_food_square_orange", "Square Orange Food Can" },
        { "can_food_square_red", "Square Red Food Can" },
        { "can_orange", "Orange Can" },
        { "can_red", "Red Can" },
        { "cereal_bar_blueberry", "Blueberry Cereal Bar" },
        { "cereal_bar_strawberry", "Strawberry Cereal Bar" },
        { "chips", "Chips" },
        { "chips_spicy", "Spicy Chips" },
        { "chocolate", "Chocolate" },
        { "chocolate_white", "White Chocolate" },
        { "cleaning_bottle_magenta", "Magenta Cleaning Bottle" },
        { "cleaning_bottle_purple", "Purple Cleaning Bottle" },
        { "cleaning_bottle_yellow", "Yellow Cleaning Bottle" },
        { "coffee_caramel", "Caramel Coffee" },
        { "coffee_latte", "Latte Coffee" },
        { "cream_blue", "Blue Cream" },
        { "cream_jar_orange", "Orange Cream Jar" },
        { "cream_jar_red", "Red Cream Jar" },
        { "cream_jar_white", "White Cream Jar" },
        { "cream_jar_yellow", "Yellow Cream Jar" },
        { "cream_red", "Red Cream" },
        { "cream_white", "White Cream" },
        { "detergent_blue", "Blue Detergent" },
        { "detergent_purple", "Purple Detergent" },
        { "detergent_red", "Red Detergent" },
        { "detergent_white", "White Detergent" },
        { "digestif", "Digestif" },
        { "dressing_golf", "Golf Dressing" },
        { "dressing_ketchup", "Ketchup Dressing" },
        { "dressing_mayo", "Mayo Dressing" },
        { "eggscarton", "Eggs Carton" },
        { "flask_coffee", "Coffee Flask" },
        { "flask_cream", "Cream Flask" },
        { "flask_green", "Green Flask" },
        { "flask_orange", "Orange Flask" },
        { "gummies", "Gummies" },
        { "gummies_sour", "Sour Gummies" },
        { "jam_black", "Black Jam" },
        { "jam_orange", "Orange Jam" },
        { "jam_pink", "Pink Jam" },
        { "jam_purple", "Purple Jam" },
        { "jam_red", "Red Jam" },
        { "jam_yellow", "Yellow Jam" },
        { "juice_apple", "Apple Juice" },
        { "juice_mango", "Mango Juice" },
        { "juice_orange", "Orange Juice" },
        { "juice_tropical", "Tropical Juice" },
        { "milk", "Milk" },
        { "milk_small", "Small Milk" },
        { "milk_small_strawberry", "Small Strawberry Milk" },
        { "milk_strawberry", "Strawberry Milk" },
        { "pizzabox", "Pizza Box" },
        { "pumpbottle_orange", "Orange Pump Bottle" },
        { "pumpbottle_purple", "Purple Pump Bottle" },
        { "pumpbottle_red", "Red Pump Bottle" },
        { "ramen", "Ramen" },
        { "ramen_spicy", "Spicy Ramen" },
        { "roll_kitchen", "Kitchen Roll" },
        { "roll_kitchen_wrapped", "Wrapped Kitchen Roll" },
        { "roll_toilet", "Toilet Roll" },
        { "roll_toilet_pack", "Toilet Roll Pack" },
        { "soda_cherry", "Cherry Soda" },
        { "soda_energy", "Energy Soda" },
        { "soda_energy_zero", "Zero Energy Soda" },
        { "soda_grape", "Grape Soda" },
        { "soda_orange", "Orange Soda" },
        { "soda_watermelon", "Watermelon Soda" },
        { "sweet_eclair", "Eclair" },
        { "sweet_marshmallow", "Marshmallow" },
        { "sweet_smores", "S'mores" },
        { "sweet_vanilla", "Vanilla Sweet" },
        { "toothpaste_blue", "Blue Toothpaste" },
        { "toothpaste_cream", "Cream Toothpaste" },
        { "toothpaste_red", "Red Toothpaste" },
        { "whisky", "Whisky" },
        { "wine_red", "Red Wine" },
        { "wine_white", "White Wine" }
    };

    public static string GetPrettyName(string technicalName)
    {
        return NameMapping.TryGetValue(technicalName, out string pretty) ? pretty : technicalName;
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("1. Select Target ScriptableObject", EditorStyles.boldLabel);
        
        // Slot to drag the ScriptableObject into
        targetLibrary = (PrefabLibrary)EditorGUILayout.ObjectField(
            "Target Library", 
            targetLibrary, 
            typeof(PrefabLibrary), 
            false
        );

        EditorGUILayout.Space();
        GUILayout.Label("2. Paste JSON GUID Array", EditorStyles.boldLabel);

        // Text area for JSON
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
        inputJson = EditorGUILayout.TextArea(inputJson, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Disable button if no library is selected
        EditorGUI.BeginDisabledGroup(targetLibrary == null);
        if (GUILayout.Button("Populate Library", GUILayout.Height(40)))
        {
            FillLibrary();
        }
        EditorGUI.EndDisabledGroup();

        if (targetLibrary == null)
        {
            EditorGUILayout.HelpBox("Please assign a PrefabLibrary to continue.", MessageType.Info);
        }
    }

    void FillLibrary()
    {
        // 1. Regex to find all GUIDs inside quotes
        Regex regex = new Regex("\"([a-fA-F0-9]+)\"");
        MatchCollection matches = regex.Matches(inputJson);

        List<PrefabLibrary.Entry> newEntries = new List<PrefabLibrary.Entry>();
        int successCount = 0;

        foreach (Match match in matches)
        {
            string guid = match.Groups[1].Value;
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!string.IsNullOrEmpty(path))
            {
                // 2. Load the actual GameObject
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    // 3. Create the Entry struct
                    PrefabLibrary.Entry entry = new PrefabLibrary.Entry();
                    entry.Prefab = prefab;
                    entry.PrettyName = GetPrettyName(prefab.name); // Use the prefab name automatically

                    newEntries.Add(entry);
                    successCount++;
                }
                else
                {
                    Debug.LogWarning($"GUID {guid} points to '{path}', which is not a GameObject. Skipping.");
                }
            }
            else
            {
                Debug.LogWarning($"GUID {guid} could not be resolved to a path.");
            }
        }

        // 4. Record Undo so you can CTRL+Z if you make a mistake
        Undo.RecordObject(targetLibrary, "Populate Prefab Library");

        // 5. Assign the new array
        targetLibrary.Entries = newEntries.ToArray();

        // 6. Mark dirty so Unity knows to save the file to disk
        EditorUtility.SetDirty(targetLibrary);
        AssetDatabase.SaveAssets();

        Debug.Log($"<color=green>Success!</color> Added {successCount} prefabs to {targetLibrary.name}.");
    }
}