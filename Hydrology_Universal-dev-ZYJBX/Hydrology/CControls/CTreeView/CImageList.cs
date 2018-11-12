using System.Drawing;
using System.Windows.Forms;

namespace Hydrology.CControls
{
    public class CImageList
    {
        public static ImageList GetImageList()
        {
            var il = new ImageList();


            //var dir = System.Environment.CurrentDirectory + "\\img\\";
            //il.Images.Add(new Bitmap(dir + "系统设置未选中.png"));  //  0
            //il.Images.Add(new Bitmap(dir + "系统设置选中.png"));  //  1
            il.Images.Add(global::Hydrology.Properties.Resources.系统设置未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.系统设置选中);

            //il.Images.Add(new Bitmap(dir + "端口设置未选中.png"));  //  2
            //il.Images.Add(new Bitmap(dir + "端口设置选中.png"));  //  3
            il.Images.Add(global::Hydrology.Properties.Resources.端口设置未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.端口设置选中);

            //il.Images.Add(new Bitmap(dir + "协议类型未选中.png"));  //  4
            //il.Images.Add(new Bitmap(dir + "协议类型选中.png"));  //  5
            il.Images.Add(global::Hydrology.Properties.Resources.协议类型未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.协议类型选中);

            //il.Images.Add(new Bitmap(dir + "信道类型未选中.png"));  //  6
            //il.Images.Add(new Bitmap(dir + "信道类型选中.png"));  //  7
            il.Images.Add(global::Hydrology.Properties.Resources.信道类型未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.信道类型选中);

            //il.Images.Add(new Bitmap(dir + "用户设置未选中.png"));  //  8
            //il.Images.Add(new Bitmap(dir + "用户设置选中.png"));  //  9
            il.Images.Add(global::Hydrology.Properties.Resources.用户设置未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.用户设置选中);




            //il.Images.Add(new Bitmap(dir + "遥测站设置未选中.png"));    //  10
            //il.Images.Add(new Bitmap(dir + "遥测站设置选中.png"));    //  11
            il.Images.Add(global::Hydrology.Properties.Resources.遥测站设置未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.遥测站设置选中);

            //il.Images.Add(new Bitmap(dir + "二级目录未选中.png"));      //  12
            //il.Images.Add(new Bitmap(dir + "二级目录选中.png"));      //  13
            il.Images.Add(global::Hydrology.Properties.Resources.二级目录未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.二级目录选中);
            //il.Images.Add(new Bitmap(dir + "三级目录未选中.png"));   //  14
            //il.Images.Add(new Bitmap(dir + "三级目录选中.png")); //  15
            il.Images.Add(global::Hydrology.Properties.Resources.三级目录未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.三级目录选中);

            //il.Images.Add(new Bitmap(dir + "四级目录未选中.png"));      //  16
            //il.Images.Add(new Bitmap(dir + "四级目录选中.png")); //  17
            il.Images.Add(global::Hydrology.Properties.Resources.四级目录未选中);
            il.Images.Add(global::Hydrology.Properties.Resources.四级目录选中);

            ////il.Images.Add(new Bitmap(dir + "四级目录未选中.png"));      //  18
            ////il.Images.Add(new Bitmap(dir + "四级目录选中.png")); //  19
            il.Images.Add(global::Hydrology.Properties.Resources.雨量);
            il.Images.Add(global::Hydrology.Properties.Resources.雨量);


            ////il.Images.Add(new Bitmap(dir + "四级目录未选中.png"));      //  20
            ////il.Images.Add(new Bitmap(dir + "四级目录选中.png")); //  21
            il.Images.Add(global::Hydrology.Properties.Resources.水位);
            il.Images.Add(global::Hydrology.Properties.Resources.水位);

            ////il.Images.Add(new Bitmap(dir + "四级目录未选中.png"));      //  22
            ////il.Images.Add(new Bitmap(dir + "四级目录选中.png")); //  23
            il.Images.Add(global::Hydrology.Properties.Resources.水文);
            il.Images.Add(global::Hydrology.Properties.Resources.水文);

            return il;
        }
    }
}
