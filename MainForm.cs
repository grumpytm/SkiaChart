namespace SkiaChart;

public partial class MainForm : Form
{
    private VisitsChart _chart;

    public MainForm()
    {
        InitializeComponent();
        _chart = new(skControl);
        SetRandoms();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        SetRandoms();
        skControl.Invalidate();
    }

    private void SetRandoms()
    {
        int[] visits = GenerateRandom(40, 60);
        int[] unique = GenerateRandom(10, 30);

        textBox1.Text = string.Join(", ", visits);
        textBox2.Text = string.Join(", ", unique);

        _chart.SetRanges(visits, unique);
    }

    private static int[] GenerateRandom(int min, int max) =>
        Enumerable.Range(0, 20).Select(x => new Random().Next(min, max + 1)).ToList().ToArray();
}