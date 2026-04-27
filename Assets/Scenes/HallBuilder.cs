using UnityEngine;

public class HallBuilder : MonoBehaviour
{
    private const float MinStatusCircleRadius = 0.56f;
    private const float MinStatusCircleThickness = 0.16f;

    [Header("Hall Size")]
    public float hallLength = 30f;
    public float hallWidth = 12f;
    public float hallHeight = 5f;

    [Header("Floor Tile Settings")]
    public float tileSize = 1f;
    public float tileThickness = 0.05f;
    public float tileGap = 0.05f;

    [Header("Box Settings")]
    public int numberOfBoxes = 4;
    public float boxWidth = 3.0f;
    public float boxHeight = 4.5f;
    public float boxDepth = 1.0f;

    [Header("Materials")]
    public Material wallMaterial;
    public Material ceilingMaterial;
    public Material floorMaterial;
    public Material boxMaterial;
    public Material boxTopMaterial;

    [Header("Box Status Indicators")]
    public float statusCircleRadius = 0.30f;
    public float statusCircleThickness = 0.16f;
    public float statusCircleHorizontalOffset = 0.84f;
    public float statusCircleMinGap = 0.25f;
    public float statusCircleVerticalOffset = 0.65f;
    public Material statusRedMaterial;
    public Material statusGreenMaterial;

    [Header("Alarm Settings")]
    public float alarmRadius = 0.35f;
    public float alarmHeight = 0.12f;
    public float alarmCenterRadius = 0.12f;
    public float alarmCenterHeight = 0.05f;
    public Material alarmMaterial;
    public Material alarmCenterMaterial;

    public Material lightMaterial;

    [Header("Ceiling Light Settings")]
    public float lightOffsetX = 3f;
    public float fixtureWidth = 1.2f;
    public float fixtureHeight = 0.08f;
    public float fixtureDepth = 0.5f;
    public float pointLightIntensity = 18f;
    public float spotLightIntensity = 28f;
    public float lightRange = 22f;
    public Material bulbMaterial;
    
    void Start()
    {
        BuildScene();
        SetupCamera();
    }

    void OnValidate()
    {
        statusCircleRadius = Mathf.Max(statusCircleRadius, MinStatusCircleRadius);
        statusCircleThickness = Mathf.Max(statusCircleThickness, MinStatusCircleThickness);

        if (!Application.isPlaying)
        {
            BuildScene();
        }
    }

    void BuildScene()
    {
        Transform oldHall = transform.Find("Hall");
        if (oldHall != null)
        {
            if (Application.isPlaying)
            {
                Destroy(oldHall.gameObject);
            }
            else
            {
                DestroyImmediate(oldHall.gameObject);
            }
        }

        Transform hallParent = new GameObject("Hall").transform;
        hallParent.SetParent(transform, false);

        Transform tilesParent = new GameObject("Tiles").transform;
        tilesParent.SetParent(hallParent, false);

        Transform boxesParent = new GameObject("Boxes").transform;
        boxesParent.SetParent(hallParent, false);

        CreateCube(
            "FloorBase",
            new Vector3(0f, 0f, 0f),
            new Vector3(hallLength, 0.2f, hallWidth),
            floorMaterial,
            hallParent
        );

        CreateCube(
            "Ceiling",
            new Vector3(0f, hallHeight, 0f),
            new Vector3(hallLength, 0.2f, hallWidth),
            ceilingMaterial != null ? ceilingMaterial : wallMaterial,
            hallParent
        );

        CreateCube(
            "Wall_Back",
            new Vector3(0f, hallHeight / 2f, hallWidth / 2f),
            new Vector3(hallLength, hallHeight, 0.2f),
            wallMaterial,
            hallParent
        );

        CreateCube(
            "Wall_Front",
            new Vector3(0f, hallHeight / 2f, -hallWidth / 2f),
            new Vector3(hallLength, hallHeight, 0.2f),
            wallMaterial,
            hallParent
        );

        CreateCube(
            "Wall_Left",
            new Vector3(-hallLength / 2f, hallHeight / 2f, 0f),
            new Vector3(0.2f, hallHeight, hallWidth),
            wallMaterial,
            hallParent
        );

        CreateCube(
            "Wall_Right",
            new Vector3(hallLength / 2f, hallHeight / 2f, 0f),
            new Vector3(0.2f, hallHeight, hallWidth),
            wallMaterial,
            hallParent
        );

        CreateTiledFloor(tilesParent);
        CreateWallBoxes(boxesParent);
        CreateCeilingAlarm(hallParent);
        CreateCeilingLights(hallParent);
    }

    void CreateTiledFloor(Transform tilesParent)
    {
        int xCount = Mathf.FloorToInt(hallLength / tileSize);
        int zCount = Mathf.FloorToInt(hallWidth / tileSize);

        float startX = -hallLength / 2f + tileSize / 2f;
        float startZ = -hallWidth / 2f + tileSize / 2f;

        for (int x = 0; x < xCount; x++)
        {
            for (int z = 0; z < zCount; z++)
            {
                CreateCube(
                    $"Tile_{x}_{z}",
                    new Vector3(startX + x * tileSize, 0.125f, startZ + z * tileSize),
                    new Vector3(tileSize - tileGap, tileThickness, tileSize - tileGap),
                    floorMaterial,
                    tilesParent
                );
            }
        }
    }

    void CreateWallBoxes(Transform boxesParent)
    {
        float spacing = 4.5f;   // smaller = closer together
        float totalSpan = spacing * (numberOfBoxes - 1);

        float bottomHeight = boxHeight / 3f;
        float topHeight = boxHeight * 2f / 3f;

        float wallZ = hallWidth / 2f;
        float boxZ = wallZ - (boxDepth / 2f);

        // center the 4 boxes across the wall
        float startX = -totalSpan / 2f;

        for (int i = 0; i < numberOfBoxes; i++)
        {
            float xPos = startX + i * spacing;

            float effectiveRadius = Mathf.Max(statusCircleRadius, MinStatusCircleRadius);
            float effectiveOffset = Mathf.Max(
                statusCircleHorizontalOffset,
                effectiveRadius + (statusCircleMinGap * 0.5f)
            );

            GameObject group = new GameObject($"Box_{i + 1}");
            group.transform.SetParent(boxesParent, false);

            CreateCube(
                "Bottom",
                new Vector3(xPos, bottomHeight / 2f, boxZ),
                new Vector3(boxWidth, bottomHeight, boxDepth),
                boxMaterial,
                group.transform
            );

            CreateCube(
                "Top",
                new Vector3(xPos, bottomHeight + topHeight / 2f, boxZ),
                new Vector3(boxWidth, topHeight, boxDepth),
                boxTopMaterial != null ? boxTopMaterial : boxMaterial,
                group.transform
            );

            float signY = bottomHeight + (topHeight * statusCircleVerticalOffset);
            float signZ = boxZ - (boxDepth * 0.5f) - (statusCircleThickness * 0.5f) - 0.005f;

            CreateStatusCircle(
                "Status_Red",
                new Vector3(xPos - effectiveOffset, signY, signZ),
                GetStatusMaterial(statusRedMaterial, Color.red),
                group.transform
            );

            CreateStatusCircle(
                "Status_Green",
                new Vector3(xPos + effectiveOffset, signY, signZ),
                GetStatusMaterial(statusGreenMaterial, Color.green),
                group.transform
            );
        }
    }

    void CreateStatusCircle(string name, Vector3 localPos, Material mat, Transform parent)
    {
        float effectiveRadius = Mathf.Max(statusCircleRadius, MinStatusCircleRadius);
        float effectiveThickness = Mathf.Max(statusCircleThickness, MinStatusCircleThickness);

        GameObject circle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circle.name = name;
        circle.transform.SetParent(parent, false);
        circle.transform.localPosition = localPos;
        circle.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        circle.transform.localScale = new Vector3(effectiveRadius, effectiveThickness * 0.5f, effectiveRadius);

        Renderer renderer = circle.GetComponent<Renderer>();
        if (mat != null)
        {
            renderer.sharedMaterial = mat;
        }
    }

    Material GetStatusMaterial(Material providedMaterial, Color fallbackColor)
    {
        if (providedMaterial != null)
        {
            return providedMaterial;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        Material generated = new Material(shader);
        generated.color = fallbackColor;
        return generated;
    }

    void CreateCeilingAlarm(Transform hallParent)
    {
        float baseHeight = 0.15f;
        float domeHeight = 0.25f;
        float radius = 0.5f;

        float ceilingY = hallHeight;

        // Base plate (touching ceiling)
        GameObject basePlate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        basePlate.name = "AlarmBase";
        basePlate.transform.SetParent(hallParent, false);
        basePlate.transform.localPosition = new Vector3(0f, ceilingY - baseHeight / 2f, 0f);
        basePlate.transform.localScale = new Vector3(radius, baseHeight / 2f, radius);

        // Red dome (siren part)
        GameObject dome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dome.name = "AlarmDome";
        dome.transform.SetParent(hallParent, false);
        dome.transform.localPosition = new Vector3(0f, ceilingY - baseHeight - domeHeight / 2f, 0f);
        dome.transform.localScale = new Vector3(radius * 1.2f, domeHeight, radius * 1.2f);

        // Apply material
        if (alarmMaterial != null)
        {
            basePlate.GetComponent<Renderer>().sharedMaterial = alarmMaterial;
            dome.GetComponent<Renderer>().sharedMaterial = alarmMaterial;
        }
    }

   void CreateCeilingLights(Transform hallParent)
    {
        float y = hallHeight - fixtureHeight;

        CreateCeilingLightFixture(new Vector3(-lightOffsetX, y, 0f), hallParent);
        CreateCeilingLightFixture(new Vector3(lightOffsetX, y, 0f), hallParent);
    }

    void CreateCeilingLightFixture(Vector3 pos, Transform parent)
    {
        GameObject fixture = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fixture.name = "CeilingLightFixture";
        fixture.transform.SetParent(parent, false);
        fixture.transform.localPosition = pos;
        fixture.transform.localScale = new Vector3(fixtureWidth, fixtureHeight, fixtureDepth);

        if (lightMaterial != null)
            fixture.GetComponent<Renderer>().sharedMaterial = lightMaterial;

        Vector3 lightPos = new Vector3(pos.x, pos.y - (fixtureHeight * 0.5f + 0.03f), pos.z);

        GameObject pointObj = new GameObject("CeilingLight_Point");
        pointObj.transform.SetParent(parent, false);
        pointObj.transform.localPosition = lightPos;

        Light pointLight = pointObj.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.color = new Color(1f, 0.97f, 0.9f);
        pointLight.intensity = pointLightIntensity;
        pointLight.range = lightRange;
        pointLight.shadows = LightShadows.None;
        pointLight.renderMode = LightRenderMode.ForcePixel;

        GameObject spotObj = new GameObject("CeilingLight_Spot");
        spotObj.transform.SetParent(parent, false);
        spotObj.transform.localPosition = lightPos;
        spotObj.transform.rotation = Quaternion.LookRotation(Vector3.down);

        Light spotLight = spotObj.AddComponent<Light>();
        spotLight.type = LightType.Spot;
        spotLight.color = new Color(1f, 0.97f, 0.9f);
        spotLight.intensity = spotLightIntensity;
        spotLight.range = lightRange;
        spotLight.spotAngle = 100f;
        spotLight.innerSpotAngle = 65f;
        spotLight.shadows = LightShadows.Soft;
        spotLight.renderMode = LightRenderMode.ForcePixel;
    }

    void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        cam.transform.position = new Vector3(0f, 2.6f, -6.5f);
        cam.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        cam.fieldOfView = 70f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1000f;
    }

    GameObject CreateCube(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = pos;
        obj.transform.localScale = scale;

        Renderer rend = obj.GetComponent<Renderer>();
        if (mat != null)
        {
            rend.sharedMaterial = mat;
        }

        return obj;
    }
}

