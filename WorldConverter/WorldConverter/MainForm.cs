using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldConverter.Format;
using static System.Environment;

namespace WorldConverter
{
    public partial class MainForm : Form
    {
        public static MainForm Form { get; private set; }

        public bool IsExcecuting { get; set; } = false;

        public bool IsErrorCancelled { get; set; } = false;

        public MainForm()
        {
            MainForm.Form = this;

            this.InitializeComponent();
        }

        private void SelectLoadDirectory(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "変換するワールドを選択してください",
                RootFolder = SpecialFolder.MyComputer
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.LoadDirectory.Text = folderBrowserDialog.SelectedPath;
            }
            folderBrowserDialog.Dispose();
        }

        private void SelectSaveDirectory(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "保存先を選択してください",
                RootFolder = SpecialFolder.MyComputer
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.SaveDirectory.Text = folderBrowserDialog.SelectedPath;
            }
            folderBrowserDialog.Dispose();
        }

        public int GetMaxProgress()
        {
            return this.Progress.Maximum;
        }

        public void SetMaxProgress(int progress)
        {
            this.Progress.Maximum = progress;
        }

        public int GetProgressValue()
        {
            return this.Progress.Value;
        }

        public void SetProgressValue(int value)
        {
            this.Progress.Value = value;
        }

        public void AddProgressValue(int value)
        {
            this.Progress.Value += value;
        }

        private void ExcecuteConvert(object sender, EventArgs e)
        {
            if (this.IsExcecuting)
            {
                this.Log("変換実行中に処理を実行することはできません");
                return;
            }
            if (this.LoadDirectory.Text == "" || this.SaveDirectory.Text == "")
            {
                this.Log("読み込むワールドと保存先を指定してください");
                return;
            }

            this.IsExcecuting = true;
            
            this.Log("実行を開始しました");
            try
            {
                this.Convert();
            }
            catch (Exception ex)
            {
                this.Log("エラーが発生しました");
                this.Log(ex.ToString());
                this.IsExcecuting = false;
            }
        }
        
        private void Convert()
        {
            string levelPath = this.LoadDirectory.Text;
            string savePath = Path.Combine(this.SaveDirectory.Text, this.GetWorldName());
            LevelProviderManager manager = new LevelProviderManager();
            ILevelProvider provider = manager.GetProvider(levelPath);
            provider.Convert(levelPath, savePath, this.GetDimensionId());
            this.CheckFinish();
        }

        private async void CheckFinish()
        {
            while (true)
            {
                await Task.Delay(100);
                if (this.IsErrorCancelled)
                {
                    this.Log("エラーが発生しました");
                    this.IsExcecuting = false;
                    this.IsErrorCancelled = false;
                    return;
                }
                if (this.GetMaxProgress() == this.GetProgressValue())
                {
                    this.Log("変換が完了しました");
                    this.IsExcecuting = false;
                    return;
                }
            }
        }

        public void Log(string text)
        {
            this.Console.Items.Add(text);
            this.Console.TopIndex = this.Console.Items.Count - 1;
        }

        public void ClearLog()
        {
            this.Console.Items.Clear();
        }

        public string GetWorldName()
        {
            return this.WorldName.Text;
        }

        public int GetDimensionId()
        {
            return this.Dimension.SelectedIndex;
        }
    }
}
