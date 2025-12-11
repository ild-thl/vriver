using UnityEngine;
using TMPro;

public class MapReveal : MonoBehaviour
{
    public float terrainSizeX = 300f;
    public float terrainSizeZ = 300f;

    public int textureWidth = 512;
    public int textureHeight = 512;

    public int circleRadius = 15;
    public float drawUpdateDistance = 2;

    public Material mapMaterial;
    public GameObject UI_MAP;
    public TMP_Text percentageText; // assign in inspector

    public Texture2D pondMaskTexture; // white = pond, black = outside

    private Texture2D maskTexture;
    private Color32[] pixelBuffer;
    private Color32[] pondPixels;
    private Vector3 positionBuffer;

    private int revealedPixels = 0; // how many pond pixels have been revealed
    private int pondPixelCount = 0; // how many pixels belong to pond
    private bool[] revealedMask;    // track which pond pixels are revealed

    public void GenerateEmptyMask()
    {
        maskTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        pixelBuffer = new Color32[textureWidth * textureHeight];
        pondPixels = pondMaskTexture.GetPixels32();
        revealedMask = new bool[pixelBuffer.Length];

        pondPixelCount = 0;
        for (int i = 0; i < pixelBuffer.Length; i++)
        {
            pixelBuffer[i] = Color.black;
            revealedMask[i] = false;

            if (pondPixels[i].r > 128) // white-ish = pond
                pondPixelCount++;
        }

        maskTexture.SetPixels32(pixelBuffer);
        maskTexture.Apply();
        mapMaterial.SetTexture("_Mask", maskTexture);
    }

    void DrawCircle(int centerX, int centerY, int radius, Color color)
    {
        for (int y = -radius; y < radius; y++)
        {
            int dy = centerY + y;
            if (dy < 0 || dy >= textureHeight) continue;

            for (int x = -radius; x < radius; x++)
            {
                int dx = centerX + x;
                if (dx < 0 || dx >= textureWidth) continue;

                if (x * x + y * y <= radius * radius)
                {
                    int index = dy * textureWidth + dx;
                    if (index >= 0 && index < pixelBuffer.Length)
                    {
                        // only count if it's part of the pond
                        if (pondPixels[index].r > 128 && !revealedMask[index])
                        {
                            revealedMask[index] = true;
                            revealedPixels++;
                        }

                        pixelBuffer[index] = color;
                    }
                }
            }
        }
    }

    public void DrawPlayerPositionOnMask()
    {
        Vector3 playerPos = transform.position;

        int pixelX = Mathf.RoundToInt((playerPos.x / terrainSizeX) * textureWidth);
        int pixelY = Mathf.RoundToInt((playerPos.z / terrainSizeZ) * textureHeight);

        DrawCircle(pixelX, pixelY, circleRadius, Color.white);

        maskTexture.SetPixels32(pixelBuffer);
        maskTexture.Apply();

        UpdatePercentageUI();
    }

    void UpdatePercentageUI()
    {
        if (pondPixelCount == 0) return;
        

        float percent = (float)revealedPixels / pondPixelCount * 100f;
        if (percentageText != null)
        {
            Debug.Log(percent);
            percentageText.text = $"{percent:F2}% Revealed";
        }
    }

    void Start()
    {
        GenerateEmptyMask();
    }

    public void UpdateMapByDistance(float dis)
    {
        if (Vector3.Distance(positionBuffer, transform.position) >= dis)
        {
            positionBuffer = transform.position;
            DrawPlayerPositionOnMask();
        }
    }

    private void Update()
    {
        //UpdateMapByDistance(drawUpdateDistance);
    }
}
