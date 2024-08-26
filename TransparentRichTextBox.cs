using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

//---------------- Hello It's Me Amro Omran    ----------------------------- \\
//---------------- Hotmail.Home@gmail.com     ------------------------------ \\
//---------------- Visit My Youtube Channel ---------------------------------\\
//---------------- https://www.youtube.com/@Amro-Omran ----------------------\\
//---------------- Please Like And subscribe For More useful Code ---------- \\
//---------------- buy me acoffee ------------------------------------------ \\
public class TransparentRichTextBox : RichTextBox
{
    private const int WS_EX_TRANSPARENT = 0x20;

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    public TransparentRichTextBox()
    {
        SetStyle(
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);
        //SetStyle(ControlStyles.UserPaint, true);
        BackColor = Color.Transparent;
        Font = new Font(Font.FontFamily, 12);

    }


    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string lpFileName);
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
            if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
            {
                cp.ClassName = "RICHEDIT50W"; //-----------behave like richtextbox  If you want it to be TextBox change "RICHEDIT50W" to "EDIT"
            }
            else if (LoadLibrary("riched20.dll") != IntPtr.Zero)
            {
                cp.ClassName = "RichEdit20W";
            }
            return cp;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // Render text on a transparent background
        using (Brush brush = new SolidBrush(this.ForeColor))
        {
            e.Graphics.DrawString(this.Text, this.Font, brush, 0, 0);
        }

       
    }
}
