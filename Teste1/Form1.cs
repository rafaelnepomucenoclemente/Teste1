using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teste1
{
    public partial class Form1 : Form
    {
        private string tcdPath = @"C:\TClient\TCD\TItem.tcd";
        private string dataPath = @"C:\TClient\Data\";

        private Dictionary<int, string> petNames;
        private Dictionary<int, Image> petImages;
        private Dictionary<int, string> petFiles;

        public Form1()
        {
            InitializeComponent();

            petNames = new Dictionary<int, string>();
            petImages = new Dictionary<int, Image>();
            petFiles = new Dictionary<int, string>();

            LoadPets();
            CreateButtons();
        }

        private void LoadPets()
        {
            TCDManager tcdManager = new TCDManager(tcdPath);
            List<TItem> items = tcdManager.GetItemsByType(TItemType.Pet);
            foreach (TItem item in items)
            {
                if (item.TPetType == TPetType.Ride && item.Image.Length > 0)
                {
                    int petID = item.TIndex;
                    petNames.Add(petID, item.Name);
                    petFiles.Add(petID, item.FileName);

                    string imagePath = dataPath + item.Image;
                    Image image = Image.FromFile(imagePath);
                    petImages.Add(petID, image);
                }
            }
        }

        private void CreateButtons()
        {
            int x = 10;
            int y = 10;
            int spacing = 10;
            int buttonSize = 50;

            foreach (int petID in petNames.Keys)
            {
                Button button = new Button();
                button.Size = new Size(buttonSize, buttonSize);
                button.Location = new Point(x, y);
                button.Image = petImages[petID];
                button.Tag = petID.ToString();
                button.Click += new EventHandler(PetButtonClick);

                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(button, petNames[petID]);

                Controls.Add(button);

                x += buttonSize + spacing;
                if (x + buttonSize > ClientSize.Width)
                {
                    x = 10;
                    y += buttonSize + spacing;
                }
            }
        }

        private void PetButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int petID = int.Parse(button.Tag.ToString());

            string petFile = petFiles[petID];
            string petPath = dataPath + petFile;

            TPet pet = new TPet();
            pet.Read(petPath);

            PetEditForm form = new PetEditForm(pet);
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                pet.Write(petPath);
            }
        }
    }
}