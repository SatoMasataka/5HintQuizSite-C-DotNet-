using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlideShow.Models
{
　　#region アップロード画面用
    public class UploadPictModel
    {
        public List<PICT> Picts { get; set; }

        public int? SelectedId { get; set; }
    }

    #endregion
}