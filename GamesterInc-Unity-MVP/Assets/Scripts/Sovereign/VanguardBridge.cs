using UnityEngine;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;

[Serializable]
public class SovereignPayload
{
    public float biological_roi;
    public float sovereign_capital;
    public float administrative_sloth;
    public string oakley_hud_command;
    public string ui_hex_color;
}

public class VanguardBridge : MonoBehaviour
{
    private ClientWebSocket ws = new ClientWebSocket();
    private string lilyPiUrl = "ws://localhost:5005/vanguard/sync";
    private CancellationTokenSource cts = new CancellationTokenSource();

    async void Start()
    {
        Debug.Log("[VANGUARD] Initializing Handshake with Lily-Pi 5...");
        try
        {
            await ws.ConnectAsync(new Uri(lilyPiUrl), cts.Token);
            Debug.Log("[VANGUARD] 10^47 Sync Established. Listening to the Animus.");
            _ = ReceiveSovereignPulse();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SLOTH INTERFERENCE] Lily-Pi connection failed: {e.Message}");
        }
    }

    private async Task ReceiveSovereignPulse()
    {
        var buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
            
            // Parse the Python JSON from Enki AI
            SovereignPayload payload = JsonUtility.FromJson<SovereignPayload>(json);
            
            // Execute on Main Unity Thread
            MainThreadDispatcher.Enqueue(() => ProcessSovereignCommand(payload));
        }
    }

    private void ProcessSovereignCommand(SovereignPayload payload)
    {
        // 1. Update the 4D Overseer with real Biological ROI
        SovereignOverseer.Instance.biologicalROI = payload.biological_roi;
        SovereignOverseer.Instance.sovereignCapital = payload.sovereign_capital;

        // 2. Trigger the Medusa or Lumen visual states on the Oakley HUD
        if (payload.oakley_hud_command == "MEDUSA_PROTOCOL_ACTIVE")
        {
            MedusaOverlay.Instance.EngageMedusaLock(payload.ui_hex_color);
        }
        else if (payload.oakley_hud_command == "FLOW_STATE_LUMEN")
        {
            MedusaOverlay.Instance.DisengageMedusa();
        }
    }

    private void OnDestroy()
    {
        cts.Cancel();
        ws.Dispose();
    }
}
