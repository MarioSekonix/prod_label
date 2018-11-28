namespace Sekonix_pop
{
    partial class Main2
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("조직관리");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("권한관리");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("메뉴관리");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("프로그램관리", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("코드관리");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("기타", new System.Windows.Forms.TreeNode[] {
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("시스템", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("W/C 관리");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("불량기준정보관리");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("공정 관리");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("설비 관리");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("자재 관리");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("생산기준정보", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12});
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("일일생산계획");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("작업지시서출력");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("작업지시", new System.Windows.Forms.TreeNode[] {
            treeNode14,
            treeNode15});
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("작업지시");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("사용자재");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("불량실적");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("생산실적", new System.Windows.Forms.TreeNode[] {
            treeNode17,
            treeNode18,
            treeNode19});
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("POP", new System.Windows.Forms.TreeNode[] {
            treeNode13,
            treeNode16,
            treeNode20});
            this.tree_Menu = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tree_Menu
            // 
            this.tree_Menu.Dock = System.Windows.Forms.DockStyle.Left;
            this.tree_Menu.Location = new System.Drawing.Point(0, 0);
            this.tree_Menu.Name = "tree_Menu";
            treeNode1.Name = "SU0020";
            treeNode1.Text = "조직관리";
            treeNode2.Name = "SU0050";
            treeNode2.Text = "권한관리";
            treeNode3.Name = "SU0040";
            treeNode3.Text = "메뉴관리";
            treeNode4.Name = "Program";
            treeNode4.Text = "프로그램관리";
            treeNode5.Name = "SU0060";
            treeNode5.Text = "코드관리";
            treeNode6.Name = "etc";
            treeNode6.Text = "기타";
            treeNode7.Name = "Menu";
            treeNode7.Text = "시스템";
            treeNode8.Name = "PU0010";
            treeNode8.Text = "W/C 관리";
            treeNode9.Name = "PU0020";
            treeNode9.Text = "불량기준정보관리";
            treeNode10.Name = "PU0030";
            treeNode10.Text = "공정 관리";
            treeNode11.Name = "PU0040";
            treeNode11.Text = "설비 관리";
            treeNode12.Name = "PU0050";
            treeNode12.Text = "자재 관리";
            treeNode13.Name = "PU0000";
            treeNode13.Text = "생산기준정보";
            treeNode14.Name = "PU0110";
            treeNode14.Text = "일일생산계획";
            treeNode15.Name = "PU0120";
            treeNode15.Text = "작업지시서출력";
            treeNode16.Name = "PU0100";
            treeNode16.Text = "작업지시";
            treeNode17.Name = "PU0210";
            treeNode17.Text = "작업지시";
            treeNode18.Name = "PU0220";
            treeNode18.Text = "사용자재";
            treeNode19.Name = "PU0230";
            treeNode19.Text = "불량실적";
            treeNode20.Name = "PU0200";
            treeNode20.Text = "생산실적";
            treeNode21.Name = "POP";
            treeNode21.Text = "POP";
            this.tree_Menu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode21});
            this.tree_Menu.Size = new System.Drawing.Size(194, 613);
            this.tree_Menu.TabIndex = 4;
            this.tree_Menu.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1108, 613);
            this.Controls.Add(this.tree_Menu);
            this.IsMdiContainer = true;
            this.Name = "Main";
            this.Text = "Sekonix";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TreeView tree_Menu;

    }
}



