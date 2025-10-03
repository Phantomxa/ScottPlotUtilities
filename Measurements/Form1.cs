namespace Measurements
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DateTime start_time = DateTime.Now - DateTime.Now.TimeOfDay + new TimeSpan(0, 0, 1);
            poisonDateTime1.Format = DateTimePickerFormat.Custom;
            poisonDateTime1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            poisonDateTime1.Text = start_time.ToString();

            DateTime start_time2 = DateTime.Now;
            poisonDateTime2.Format = DateTimePickerFormat.Custom;
            poisonDateTime2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            poisonDateTime2.Text = start_time2.ToString();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var preform_no = tbPreformno.Text;

            var connection = new MsSqlConnectionManager();

            var query = Queries.CoatingGeometry(preform_no);

            var dtCoat = connection.Connect(query);


        }
    }
}
