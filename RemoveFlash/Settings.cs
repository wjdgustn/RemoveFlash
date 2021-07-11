using UnityEngine;
using UnityModManagerNet;

namespace RemoveFlash {
    public class MainSettings : UnityModManager.ModSettings, IDrawable {
        [Draw("실패시 빨간 플래시 제거")] public bool FailRedFlash = true;
        [Draw("실패시 하얀 플래시 제거")] public bool FailWhiteFlash = true;
        [Draw("체크포인트 플래시 제거")] public bool CheckpointFlash = true;
        [Draw("클리어 플래시 제거")] public bool ClearFlash = true;

        public override void Save(UnityModManager.ModEntry modEntry) {
            UnityModManager.ModSettings.Save(this, modEntry);
        }
        
        public void OnChange() {
            
        }
        
        public void OnGUI(UnityModManager.ModEntry modEntry) {
            Main.Settings.Draw(modEntry);
        }

        public void OnSaveGUI(UnityModManager.ModEntry modEntry) {
            Main.Settings.Save(modEntry);
        }
    }
}