using System;
using System.Windows.Forms;
using WorldConverter.Format;

namespace WorldConverter
{
    public partial class MainForm : Form
    {
        public static MainForm Form { get; private set; }

        public bool IsExcecuting { get; set; } = false;

        public MainForm()
        {
            MainForm.Form = this;

            this.InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SelectLoadDirectory(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "変換するワールドを選択してください",
                RootFolder = System.Environment.SpecialFolder.MyComputer
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
                RootFolder = System.Environment.SpecialFolder.MyComputer
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.SaveDirectory.Text = folderBrowserDialog.SelectedPath;
            }
            folderBrowserDialog.Dispose();
        }

        public void SetMaxProgress(int progress)
        {
            this.Progress.Maximum = progress;
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
            this.IsExcecuting = true;

            this.ClearLog();
            this.Log("実行を開始しました");
            this.Convert();
        }
        
        private void Convert()
        {
            string path = this.LoadDirectory.Text;
            LevelProviderManager manager = new LevelProviderManager();
            ILevelProvider provider = manager.GetProvider(path);
            provider.ConvertAsync(path, $"{this.SaveDirectory.Text}\\{this.WorldName.Text}");
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
    }
}
