//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SlideShow.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ANSWER_HIST
    {
        public int QUIZ_ID { get; set; }
        public int USER_ID { get; set; }
        public System.DateTime ANSWER_DT { get; set; }
        public Nullable<int> SCORE { get; set; }
        public int FIRST_ANSWER_FLG { get; set; }
    
        public virtual QUIZ QUIZ { get; set; }
    }
}
