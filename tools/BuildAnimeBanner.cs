using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

public static class BuildAnimeBanner {
  static PropertyItem Prop(int id, int type, byte[] value) {
    var p=(PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
    p.Id=id;p.Type=(short)type;p.Len=value.Length;p.Value=value;return p;
  }
  static Bitmap Frame(Image src,int i,int count) {
    int w=1000,h=430; var b=new Bitmap(w,h,PixelFormat.Format24bppRgb);
    using(var g=Graphics.FromImage(b)) {
      g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      g.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
      double phase=Math.Sin(i*Math.PI*2/count), zoom=1.01+0.012*(phase+1)/2;
      int dw=(int)(w*zoom),dh=(int)(h*zoom),dx=(w-dw)/2+(int)(phase*3),dy=(h-dh)/2+(int)(phase*-3);
      g.DrawImage(src,new Rectangle(dx,dy,dw,dh));
      using(var shade=new SolidBrush(Color.FromArgb(92,5,5,20))) g.FillRectangle(shade,570,0,430,h);
      using(var shade2=new SolidBrush(Color.FromArgb(95,3,4,17))) g.FillRectangle(shade2,0,365,w,65);
      using(var pen=new Pen(Color.FromArgb(185,238,190,242),2)) g.DrawRectangle(pen,14,14,w-29,h-29);
      using(var glow=new SolidBrush(Color.FromArgb(235,249,168,212))) g.FillEllipse(glow,624,76,8,8);
      using(var small=new Font("Consolas",9,FontStyle.Bold)) using(var br=new SolidBrush(Color.FromArgb(240,249,168,212))) g.DrawString("NEW CHARACTER ACQUIRED",small,br,650,72);
      using(var name=new Font("Georgia",31,FontStyle.Bold)) using(var br=new SolidBrush(Color.White)) g.DrawString("Samuel Sukarno",name,br,648,111);
      using(var role=new Font("Consolas",11,FontStyle.Bold)) using(var br=new SolidBrush(Color.FromArgb(255,165,243,252))) g.DrawString("FRONT-END ENGINEER",role,br,651,164);
      using(var info=new Font("Consolas",9,FontStyle.Regular)) using(var br=new SolidBrush(Color.FromArgb(235,226,232,240))) {g.DrawString("PATH     THE ARCHITECT",info,br,651,220);g.DrawString("REGION   JAKARTA",info,br,651,247);g.DrawString("PLAYER   @FANSEMS1",info,br,651,274);}
      using(var quote=new Font("Georgia",11,FontStyle.Italic)) using(var br=new SolidBrush(Color.FromArgb(235,233,213,255))) g.DrawString("Turning ideas into experiences people love.",quote,br,651,322);
      var rnd=new Random(911+i*7); using(var petal=new SolidBrush(Color.FromArgb(210,255,205,229))) for(int n=0;n<12;n++){int px=(rnd.Next(w)+i*31)%w,py=(rnd.Next(h)+i*23)%h;g.FillEllipse(petal,px,py,3+rnd.Next(4),7+rnd.Next(6));}
    } return b;
  }
  public static void Run(string input,string output) {
    using(var src=Image.FromFile(input)) {
      int count=14; var frames=new Bitmap[count]; for(int i=0;i<count;i++) frames[i]=Frame(src,i,count);
      var delay=new byte[count*4]; for(int i=0;i<count;i++) Array.Copy(BitConverter.GetBytes(9),0,delay,i*4,4);
      frames[0].SetPropertyItem(Prop(0x5100,4,delay)); frames[0].SetPropertyItem(Prop(0x5101,3,new byte[]{0,0}));
      var codec=Array.Find(ImageCodecInfo.GetImageEncoders(),c=>c.MimeType=="image/gif");
      var ep=new EncoderParameters(1); ep.Param[0]=new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag,(long)EncoderValue.MultiFrame); frames[0].Save(output,codec,ep);
      ep.Param[0]=new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag,(long)EncoderValue.FrameDimensionTime); for(int i=1;i<count;i++) frames[0].SaveAdd(frames[i],ep);
      ep.Param[0]=new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag,(long)EncoderValue.Flush); frames[0].SaveAdd(ep);
      foreach(var f in frames) f.Dispose();
    }
  }
}
