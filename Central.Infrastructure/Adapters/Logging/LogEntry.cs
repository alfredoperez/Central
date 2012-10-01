//New BSD License (BSD)
//Copyright ©2012, Alfredo Perez
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
//following conditions are met:

//* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

//* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following
//disclaimer in the documentation and/or other materials provided with the distribution.

//* Neither the name of Alfredo Perez nor the names of its contributors may be used to endorse or promote products
//derived from this software without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
//STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Central.Infrastructure.Adapters.Logging
{
  public class LogEntry
  {
    public enum ImageType
    {
      Debug = 0,
      Error = 1,
      Fatal = 2,
      Info = 3,
      Warn = 4,
      Custom = 5
    }

    private static readonly Dictionary<ImageType, BitmapSource> ImageList =
        new Dictionary<ImageType, BitmapSource>
          {
                {ImageType.Debug, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Question.Handle, Int32Rect.Empty, null)},
                {ImageType.Error, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, null)},
                {ImageType.Fatal, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Hand.Handle, Int32Rect.Empty, null)},
                {ImageType.Info, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, null)},
                {ImageType.Warn, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, null)},
                {ImageType.Custom, Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Asterisk.Handle, Int32Rect.Empty, null)}
            };

    public static BitmapSource Images(ImageType type)
    {
      return ImageList[type];
    }

    public int Item { get; set; }

    private DateTime _timeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public DateTime TimeStamp
    {
      get { return _timeStamp; }
      set { _timeStamp = value; }
    }

    private BitmapSource _image = ImageList[ImageType.Custom];

    public BitmapSource Image
    {
      get { return _image; }
      set { _image = value; }
    }

    public string Level { get; set; }

    public string Thread { get; set; }

    public string Message { get; set; }

    public string MachineName { get; set; }

    public string UserName { get; set; }

    public string HostName { get; set; }

    public string App { get; set; }

    public string Throwable { get; set; }

    public string Class { get; set; }

    public string Method { get; set; }

    public string File { get; set; }

    public LogEntry()
    {
      Line = string.Empty;
      File = string.Empty;
      Method = string.Empty;
      Class = string.Empty;
      Throwable = string.Empty;
      App = string.Empty;
      HostName = string.Empty;
      UserName = string.Empty;
      MachineName = string.Empty;
      Message = string.Empty;
      Thread = string.Empty;
      Level = string.Empty;
      Item = 0;
    }

    public string Line { get; set; }
  }
}