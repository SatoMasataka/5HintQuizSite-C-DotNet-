using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlideShow.Models.ViewModel
{
    public class MakeQuizModel
    {

        [DisplayName("クイズのタイトル")]
        [Required(ErrorMessage = "クイズのタイトルが入力されていません。")]
        public string QuizName { get; set; }
        
        [DisplayName("クイズの説明")]
        [Required(ErrorMessage = "クイズの説明が入力されていません。")]
        public string QuizDetail { get; set; }

        [DisplayName("カテゴリ")]
        [Required(ErrorMessage = "カテゴリが入力されていません。")]
        public int CategoryId { get; set; }
        
        [DisplayName("正答")]
        [Required(ErrorMessage = "正答が入力されていません。")]
        public string Answer { get; set; }
        
        [DisplayName("正答(ひらがな)")]
        [Required(ErrorMessage = "正答(ひらがな)が入力されていません。")]
        public string AnswerHiragana { get; set; }
        
        [DisplayName("解説")]
        [Required(ErrorMessage = "解説が入力されていません。")]
        public string AnswerDetail { get; set; }

        /// <summary>
        /// カテゴリリストへのバインドデータ
        /// </summary>
        public List<SelectListItem> CategoryList { get; set; }

        /// <summary>
        /// 画像リストへのバインドデータ
        /// </summary>
        //public List<SelectListItem> PictList { get; set; }

        /// <summary>
        /// 5ヒント格納用
        /// </summary>
        [DisplayName("ヒント")]
        public List<Hint> HintList { get; set; }

        /// <summary>
        /// クイズID
        /// </summary>
        public int? QuizId { get; set; }

        public List<PICT> Picts { get; set; }

        public int? SelectedId { get; set; }
    }

    public class Hint
    {
        [Range(1, int.MaxValue, ErrorMessage = "ヒント画像が選択されていません。")]
        public int PictId { get; set; }

        public string PictName { get; set; }

        public string HintTxt { get; set; }

        public string PictPath { get; set; }
    }
}