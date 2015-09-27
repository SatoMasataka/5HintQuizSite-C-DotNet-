using SlideShow.Models;
using SlideShow.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlideShow.Controllers
{
    public class SlideShowController : Controller
    {
        /// <summary>
        /// ユーザーホーム
        /// </summary>
        /// <returns></returns>
        public ActionResult UsersHome()
        {
            Session["UserId"] = 1;
            return View();
        }

        public ActionResult SkitterSlide ()
        {
            return View();
        }

        /// <summary>
        /// 作ったクイズ一覧
        /// </summary>
        /// <returns></returns>
        public ActionResult MadeQuizList()
        {
            Session["UserId"] = 1;
            MadeQuizListModel model = new MadeQuizListModel();
            int userId = (int)Session["UserId"];

            //ユーザー作成のクイズを取得
            using (var db = new SakanaDBEntities1())
            {
                model.MadeQuizList = (from q in db.QUIZ
                                      where q.USER_ID == userId
                                      && q.DELETE_FLG == 0
                                      join c in db.CATEGORY on q.CATEGORY_ID equals c.CATEGORY_ID
                                      select new QuizData()
                                      {
                                          Quiz=q,
                                          CategoryName=c.CATEGORY_NAME
                                      }).ToList<QuizData>();
            }
            return View(model);
        }

        /// <summary>
        /// クイズ編集(クイズ作成画面)への遷移
        /// </summary>
        /// <param name="QuizId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GoToEdit(string QuizId)
        {
            int userId = (int)Session["UserId"];
            MakeQuizModel model = new MakeQuizModel();
            int qId = int.Parse(QuizId);

            model.QuizId = qId;

            //選択されたクイズデータをモデルへ
            using (var db = new SakanaDBEntities1())
            {
                model = (from q in db.QUIZ
                        where q.USER_ID == userId
                        && q.DELETE_FLG == 0
                        && q.QUIZ_ID==qId
                        select new MakeQuizModel()
                        {
                            QuizName = q.QUIZ_NAME,
                            QuizDetail =q.QUIZ_DETAIL,
                            CategoryId = (int)q.CATEGORY_ID,
                            Answer = q.ANSWER,
                            AnswerHiragana = q.ANSWER_HIRAGANA,
                            AnswerDetail=q.ANSWER_DETAIL,
                        }).First();

               
            }
            model.HintList = GetHints(qId);

            //カテゴリリストの項目取得
            model.CategoryList = MakeCategoryList();

            //画像リストの項目取得
            //model.PictList = MakePictList();

            return View("MakeQuiz", model);     
        }


        /// <summary>
        /// クイズ作成
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MakeQuiz() 
        {
            MakeQuizModel model = new MakeQuizModel();

            //カテゴリリストの項目取得
            model.CategoryList = MakeCategoryList(); 

            //画像リストの項目取得
            //model.PictList = MakePictList();
            model.Picts = SearchPictData();

        
            return View("MakeQuiz",model);         
        }

        /// <summary>
        /// 画像選択ダイアログ
        /// </summary>
        /// <returns></returns>
        public ActionResult _SelectPict()
        {
            //Ajaxではないときはお引取り願う
            if (!Request.IsAjaxRequest())
                return new EmptyResult();

            UploadPictModel mdl = new UploadPictModel();
            mdl.Picts = SearchPictData();
            return PartialView("_SelectPict", mdl);
        }

        [HttpPost]
        public ActionResult MakeQuiz(MakeQuizModel postedModel)
        {
            if (!ModelState.IsValid)
            {
                //validに引っかかったら元の画面へ

                //カテゴリリストの項目取得
                postedModel.CategoryList = MakeCategoryList();

                //画像リストの項目取得
                //postedModel.PictList = MakePictList();
                return View(postedModel);
            }

            //クイズ登録
            RegisterQuiz(postedModel);
            return Redirect("MadeQuizList");
        }


       

        /// <summary>
        /// クイズ登録
        /// </summary>
        /// <param name="postedModel"></param>
        private void RegisterQuiz(MakeQuizModel postedModel)
        {
            using (var db = new SakanaDBEntities1())
            {
                //新規登録
                if (postedModel.QuizId == null)
                {
                    //クイズテーブルへのインサート
                    var inserted = db.QUIZ.Add(new QUIZ()
                    {
                        QUIZ_NAME = postedModel.QuizName,
                        QUIZ_DETAIL = postedModel.QuizDetail,
                        USER_ID = (int)Session["UserId"],
                        CHALLENGE_NUM = 0,
                        CLEAR_NUM = 0,
                        CATEGORY_ID = postedModel.CategoryId,
                        ANSWER = postedModel.Answer,
                        ANSWER_HIRAGANA = postedModel.AnswerHiragana,
                        ANSWER_DETAIL = postedModel.AnswerDetail,
                        CREATED_DT = DateTime.Now,
                        EDITED_DT = DateTime.Now,
                        DELETE_FLG = 0
                    });

                    //クイズ画像テーブルへのインサート
                    int i = 1;
                    foreach (var hint in postedModel.HintList)
                    {

                        db.QUIZ_PICT.Add(new QUIZ_PICT()
                        {
                            QUIZ_ID = inserted.QUIZ_ID,
                            HINT_SEQ = i,
                            PICT_ID = hint.PictId,
                            HINT_TXT = hint.HintTxt
                        });

                        i++;
                    }
                }
                //編集
                else 
                {
                    //クイズテーブル更新
                    var targetQUIZ = db.QUIZ.Single(q => q.QUIZ_ID==postedModel.QuizId);
                    targetQUIZ.QUIZ_NAME = postedModel.QuizName;
                    targetQUIZ.QUIZ_DETAIL = postedModel.QuizName;
                    targetQUIZ.CATEGORY_ID = postedModel.CategoryId;
                    targetQUIZ.ANSWER = postedModel.Answer;
                    targetQUIZ.ANSWER_HIRAGANA = postedModel.AnswerHiragana;
                    targetQUIZ.ANSWER_DETAIL = postedModel.AnswerDetail;
                    targetQUIZ.EDITED_DT = DateTime.Now;

                    //クイズ画像テーブル更新
                    int i = 1;
                    foreach (var hint in postedModel.HintList)
                    {
                        var targetQUIZPIC = db.QUIZ_PICT.Single
                                               (q => q.QUIZ_ID == postedModel.QuizId　
                                               && q.HINT_SEQ==i);
                        targetQUIZPIC.PICT_ID= hint.PictId;
                        targetQUIZPIC.HINT_TXT = hint.HintTxt;                       

                        i++;
                    }
                }
                db.SaveChanges(); 
            }
        }



        //画像アップロード
        [HttpGet]
        public ActionResult UploadPict()
        {
            UploadPictModel mdl = new UploadPictModel();
            mdl.Picts = SearchPictData();
            return View(mdl);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPictPost(HttpPostedFileBase fl, string picTitle)
        {
            //仮想パス(DB保存用)
            string upfileKasoPath;
            string pictTitle = picTitle;
            const string uploadFolder = "../N/";

            UploadPictModel mdl = new UploadPictModel();
            mdl.Picts = SearchPictData();

            // コンテンツ・タイプが「image/*」であるか（画像ファイルか）
            // をチェック
            if (fl.ContentType.StartsWith("image/"))
            {

                // アップロード先のパスを生成
                upfileKasoPath = uploadFolder + FileNameWithTimestamp(fl.FileName);
                string upfileZettaiPath = Server.MapPath(uploadFolder) + FileNameWithTimestamp(fl.FileName);

                // 同名のファイルが存在する場合はエラー
                if (System.IO.File.Exists(upfileZettaiPath))
                {
                    ViewBag.Msg = "同名のファイルが存在します。";
                    return View("UploadPict", mdl);
                }
                else
                {
                    // 画像ファイルで同名のファイルが存在しない場合は保存処理
                    fl.SaveAs(upfileZettaiPath);
                    ViewBag.Msg = String.Format(
                      "{0}をアップロードしました。", fl.FileName);
                }
            }
            else
            {
                // 画像ファイルでない場合はエラー
                ViewBag.Msg = "画像以外はアップロードできません。";
                return View("UploadPict", mdl);
            }

            Session["UserId"] = 1;
            int userId = (int)Session["UserId"];

            //DB登録処理
           using (var db = new SakanaDBEntities1())
           {
               db.PICT.Add(new PICT 
               { 
                   CREATE_DT=DateTime.Now,
                   EDIT_DT = DateTime.Now,
                   DELETE_FLG=0,
                   PICT_PATH=upfileKasoPath,
                   USER_ID = userId,
                   PICT_TITLE=pictTitle
               });
          
               // SaveChangesが呼び出された段階で初めてInsert文が発行される
               db.SaveChanges();              
           }

           ViewBag.Msg = "画像アップロードしました。";
            // 入力元のフォームに結果を表示
           return View("UploadPict", mdl);
        }

        /// <summary>
        /// クイズ詳細
        /// </summary>
        /// <param name="picId"></param>
        /// <returns></returns>
        public ActionResult QuizDetail(string quizId) 
        {
            Session["UserId"] = 1;
            QuizData model = new QuizData();
            int userId = (int)Session["UserId"];
            
            try
            {
                int qId = int.Parse(quizId);
                model = GetQuizData(qId);
            }
            catch
            {
                return View("Error");
            }

                return View(model);            
        }

        /// <summary>
        /// クイズをやる
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DoQuiz(string quizId) 
        {
            DoQuizModel model = new DoQuizModel();
            try
            {
                int qId = int.Parse(quizId);
                model.quizData = GetQuizData(qId);

                model.quizHints = GetHints(qId);

            }
            catch 
            { 
                return View("Error");
            }

            return View(model);     

        }

        //答え合わせ
        [HttpPost]
        public ActionResult KotaeAwase(string quizId, string answer) 
        {
            KotaeAwaseModel model = new KotaeAwaseModel();
            try
            {
                int qId = int.Parse(quizId);
                model.quizData = GetQuizData(qId);

                if (model.quizData.Quiz.ANSWER_HIRAGANA == answer)
                {
                    ViewBag.Mes = "正解！";
                }
                else 
                {
                    ViewBag.Mes = "はずれ";
                }

            }
            catch
            {
                return View("Error");
            }
            return View(model);     
        }


        #region Common

        /// <summary>
        /// クイズ情報
        /// </summary>
        /// <param name="qId"></param>
        /// <returns></returns>
        private QuizData GetQuizData(int qId) 
        {
            QuizData retData;
            using (var db = new SakanaDBEntities1())
            {
                retData = (from q in db.QUIZ
                         where q.QUIZ_ID == qId
                         && q.DELETE_FLG == 0
                         join c in db.CATEGORY on q.CATEGORY_ID equals c.CATEGORY_ID
                         join u in db.USERS on q.USER_ID equals u.USER_ID
                         select new QuizData()
                         {
                             Quiz = q,
                             CategoryName = c.CATEGORY_NAME,
                             UserName = u.USER_NAME
                         }).ToList<QuizData>().First();
            }

            return retData;
        }

        /// <summary>
        /// ヒント
        /// </summary>
        /// <returns></returns>
        private List<Hint> GetHints (int qId)
        {
            List<Hint> retList;
            using (var db = new SakanaDBEntities1())
            {

               retList = (from q in db.QUIZ_PICT
                                  where q.QUIZ_ID == qId
                                  join j in db.PICT on q.PICT_ID equals j.PICT_ID
                                  orderby q.HINT_SEQ
                                  select new Hint()
                                  {
                                      HintTxt = q.HINT_TXT,
                                      PictId = q.PICT_ID,
                                      PictName=j.PICT_TITLE,
                                      PictPath=j.PICT_PATH
                                  }).ToList<Hint>();
            }
            return retList;
        }

        /// <summary>
        /// PICTから該当ユーザーの画像データを取得
        /// </summary>
        /// <returns></returns>
        private List<PICT> SearchPictData() 
        {
            List<PICT> retList=new List<PICT>();
            using (var db = new SakanaDBEntities1())
            {
               retList = (from b in db.PICT
                            where b.USER_ID == 1
                            && b.DELETE_FLG==0
                            select b).ToList<PICT>();
        
            }
            return retList;
        }


        /// <summary>
        /// 画像リストボックスバインドデータ
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> MakePictList()
        {
            return (from p in SearchPictData()
                    select new SelectListItem()
                    {
                        Value = p.PICT_ID.ToString(),
                        Text = p.PICT_TITLE
                    }).ToList<SelectListItem>();
         }

        /// <summary>
        /// カテゴリリストボックスバインドデータ
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> MakeCategoryList() 
        {
            //カテゴリリストの項目取得
            using (var db = new SakanaDBEntities1())
            {
                return (from q in db.CATEGORY
                        where q.DELETE_FLG == 0
                        select new SelectListItem()
                        {
                            Value = q.CATEGORY_ID.ToString(),
                            Text = q.CATEGORY_NAME
                        }).ToList<SelectListItem>();
            }
        }

        /// <summary>
        /// タイムスタンプつきファイル名に変換
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string FileNameWithTimestamp(string filePath) 
        {
            string fPathWithTime= Path.GetFileNameWithoutExtension(filePath) +DateTime.Now.ToString("yyyyMMddhhssmmfff");
            string extention = Path.GetExtension(filePath);
            return fPathWithTime + "." + extention;
        }
        #endregion

    }
}