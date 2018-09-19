using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace BugRestore
{
    public partial class Form1 : Form
    {
        private StringBuilder log = new StringBuilder();
        private int oldLogLength;
        private int actionstartid = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ActionStartID"]);
        private int filestartid = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ActionStartID"]);
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 500;
            timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //using (StreamReader sr = new StreamReader(File.OpenRead("C:\\Users\\gxu\\Desktop\\123.html")))
            //{

            //    var str = sr.ReadToEnd();
            //    Bug bug = new Bug();

            //    var stepsstartindex = Regex.Match(str, $@"<ol id='historyItem'>").Index;
            //    if (stepsstartindex > 0)
            //    {
            //        var stepsendindex = str.IndexOf("</ol>", stepsstartindex);
            //        SetAction(bug,str.Substring(stepsstartindex, stepsendindex - stepsstartindex + 5));
            //    }

            //}
            SetFile(new Bug() { id = 1813 }, new DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["FilePath"]));
            //this.backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["FilePath"]);
            List<Bug> bugs = new List<Bug>();
            var contentdivLength = "<div class='content'>".Length;

            foreach (var item in directory.GetFiles())
            {
                if (Regex.IsMatch(item.Name, @"bug-view-\d{3,6}.html"))
                {
                    using (StreamReader sr = new StreamReader(item.OpenRead()))
                    {
                        try
                        {
                            var str = sr.ReadToEnd();
                            Bug bug = new Bug();
                            bug.id = Convert.ToInt32(Regex.Match(item.Name, @"bug-view-(\d{3,6}).html").Groups[1].Value);
                            bug.title = Regex.Match(str, $@"<strong>{bug.id.ToString()}</strong></span>\n\s*<strong>(.*)</strong>").Groups[1].Value;
                            var stepsstartindex = Regex.Match(str, $@"<div class='content'>(.*)").Index;
                            if (stepsstartindex > 0)
                            {
                                var stepsendindex = str.IndexOf("</div>", stepsstartindex);
                                bug.steps = str.Substring(stepsstartindex + contentdivLength,
                                    stepsendindex - (stepsstartindex + contentdivLength)).Replace("'","''");
                            }
                            bug.severity = Convert.ToInt32(Regex.Match(str, @"<th>严重程度</th>\n\s*<td><strong>(\d)</strong>").Groups[1].Value);
                            bug.type = GetType(Regex.Match(str, $@"<th>Bug类型</th>\n\s*<td>(.*)</td>").Groups[1].Value);
                            bug.status = GetStatus(Regex.Match(str, $@"<th>Bug状态</th>\n\s*<td.*><strong>(.*)</strong></td>").Groups[1].Value);
                            bug.confirmed = GetConfirmed(Regex.Match(str, $@"<th>是否确认</th>\n\s*<td>(.*)</td>").Groups[1].Value);
                            bug.activatedCount = Convert.ToInt32(Regex.Match(str, @"<th>激活次数</th>\n\s*<td>(.*)</td>").Groups[1].Value);
                            var openbymatch = Regex.Match(str, $@"<th.*>由谁创建</th>\n\s*<td>(.*)于(.*)</td>").Groups;
                            if (openbymatch.Count > 2)
                            {
                                bug.openedBy = GetAccount(openbymatch[1].Value.Trim());
                                bug.openedDate = Convert.ToDateTime(openbymatch[2].Value.Trim());
                            }
                            var assignedmatch = Regex.Match(str, $@"<th.*>当前指派</th>\n\s*<td>(.*)于(.*)</td>").Groups;
                            if (assignedmatch.Count > 2)
                            {
                                bug.assignedTo = GetAccount(assignedmatch[1].Value.Trim());
                                bug.assignedDate = Convert.ToDateTime(assignedmatch[2].Value.Trim());
                            }
                            var resolvedmatch = Regex.Match(str, $@"<th.*>由谁解决</th>\n\s*<td>(.*)于(.*)</td>").Groups;
                            if (resolvedmatch.Count > 2)
                            {
                                bug.resolvedBy = GetAccount(resolvedmatch[1].Value.Trim());
                                bug.resolvedDate = Convert.ToDateTime(resolvedmatch[2].Value.Trim());
                            }
                            bug.resolution = GetResolution(Regex.Match(str, $@"<th.*>解决方案</th>\n\s*<td>\n\s*(.*)</td>").Groups[1].Value);
                            var closedmatch = Regex.Match(str, $@"<th.*>由谁关闭</th>\n\s*<td>(.*)于(.*)</td>").Groups;
                            if (closedmatch.Count > 2)
                            {
                                bug.closedBy = GetAccount(closedmatch[1].Value.Trim());
                                bug.closedDate = Convert.ToDateTime(closedmatch[2].Value.Trim());
                            }
                            var lastEditedmatch = Regex.Match(str, $@"<th.*>最后修改</th>\n\s*<td>(.*)于(.*)</td>").Groups;
                            if (lastEditedmatch.Count > 2)
                            {
                                bug.lastEditedBy = GetAccount(lastEditedmatch[1].Value.Trim());
                                bug.lastEditedDate = Convert.ToDateTime(lastEditedmatch[2].Value.Trim());
                            }

                            //var stepsstartindex = Regex.Match(str, $@"<div class='content'>(.*)").Index;
                            //if (stepsstartindex > 0)
                            //{
                            //    var stepsendindex = str.IndexOf("</div>", stepsstartindex);
                            //    bug.steps = str.Substring(stepsstartindex + contentdivLength,
                            //        stepsendindex - (stepsstartindex + contentdivLength)).Replace("'", "''");
                            //}

                            bugs.Add(bug);
                        }
                        catch(Exception ex)
                        {
                            log.AppendLine(ex.Message + ex.StackTrace);
                            return;
                        }
                    }
                }
            }

            log.AppendLine(DateTime.Now.ToShortTimeString() + ":生成成功 路径:" + BuildSqlFile(bugs));
        }


        private string BuildSqlFile(List<Bug> bugs)
        {
            var sqlpath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["FilePath"],
                DateTime.Now.ToString("yyyyMMdd HHmmss") + ".sql");
            using (StreamWriter sr = new StreamWriter(
                File.Create(sqlpath), Encoding.UTF8))
            {
                foreach (var bug in bugs)
                {
                    var sqlstr = $@"delete from zt_bug where id={bug.id};
                    insert into zt_bug(id,product,branch,module,project,plan,story,
                    storyVersion,task,toTask,toStory,title,keywords,severity,pri,
                    type,os,browser,hardware,found,steps,`status`,color,confirmed,
                    activatedCount,mailto,openedBy,openedDate,openedBuild,assignedTo,
                    assignedDate,resolvedBy,resolution,resolvedBuild,resolvedDate,
                    closedBy,closedDate,duplicateBug,linkBug,`case`,caseVersion,result,
                    testtask,lastEditedBy,lastEditedDate,deleted)
                    values({bug.id},{bug.product},{bug.branch},{bug.module},{bug.project},{bug.plan},{bug.story},
                    {bug.storyVersion},{bug.task},{bug.toTask},{bug.toStory},'{bug.title}','{bug.keywords}',{bug.severity},{bug.pri},
                    '{bug.type}','{bug.os}','{bug.browser}','{bug.hardware}','{bug.found}','{bug.steps}','{bug.status}','{bug.color}',{bug.confirmed},
                    {bug.activatedCount},'{bug.mailto}','{bug.openedBy}','{bug.openedDate}','{bug.openedBuild}','{bug.assignedTo}',
                    '{bug.assignedDate}','{bug.resolvedBy}','{bug.resolution}','{bug.resolvedBuild}','{bug.resolvedDate}',
                    '{bug.closedBy}','{bug.closedDate}',{bug.duplicateBug},'{bug.linkBug}',{bug.Case},{bug.caseVersion},{bug.result},
                    {bug.testtask},'{bug.lastEditedBy}','{bug.lastEditedDate}','0');";
                    sr.WriteLine(sqlstr);
                }
            }
            return sqlpath;
        }

        private void SetAction(Bug bug,string Hisstr)
        {
            XmlDocument xmlDocument = new XmlDocument();
            Hisstr = Regex.Replace(Hisstr, @"(class='article-content .*')", "class='article-content'");
            xmlDocument.LoadXml(Hisstr);
            XmlNodeList xmlNodes = xmlDocument.SelectNodes("/ol/li");

            for (int i = 0; i < xmlNodes.Count; i++)
            {
                var node = xmlNodes.Item(i);
                Action action = new Action()
                {
                    id = actionstartid++,
                    objectID = bug.id,
                };
                XmlNode spannode = node.SelectNodes("span").Item(0);
                XmlNodeList oplist = spannode.SelectNodes("strong");
                action.actor = GetAccount(oplist.Item(0).InnerText);
                action.date = Convert.ToDateTime(Regex.Match(spannode.InnerText, @"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2})").Groups[1].Value);
                if (spannode.InnerText.Contains("创建"))
                {
                    action.action = "opened";
                }
                else if (spannode.InnerText.Contains("指派给"))
                {
                    action.action = "assigned";
                    action.comment = node.SelectNodes(".//div[@class='article-content']").Item(0)?.InnerXml;
                    History history = new History();
                    history.action = 32;
                    history.field = "assignedTo";
                    var assignedMap = Regex.Match(node.SelectNodes("div").Item(0).InnerText, "旧值为 \"(.*)\"，新值为 \"(.*)").Groups;
                    history.old = assignedMap[1].Value;
                    history.New = assignedMap[2].Value;
                    action.Histories.Add(history);
                }
                else if (spannode.InnerText.Contains("解决"))
                {
                    action.action = "resolved";
                    action.comment = node.SelectNodes(".//div[@class='article-content']").Item(0)?.InnerXml;
                    action.extra = GetResolution(oplist.Item(1).InnerText);
                }
                else if (spannode.InnerText.Contains("添加备注"))
                {
                    action.action = "commented";
                    action.comment = node.SelectNodes(".//div[@class='article-content']").Item(0)?.InnerXml;
                }
                else if (spannode.InnerText.Contains("激活"))
                {
                    action.action = "activated";
                    action.comment = node.SelectNodes(".//div[@class='article-content']").Item(0)?.InnerXml;
                }
                else
                {
                    //不支持的日志
                    continue;
                }
                bug.Actions.Add(action);
            }
        }

        private void SetFile(Bug bug, DirectoryInfo directory)
        {
            var dir = directory.GetDirectories($"{bug.id}").FirstOrDefault();

            if (dir != null)
            {
                foreach (var item in dir.GetFiles())
                {
                    AFile aFile = new AFile();
                    aFile.id = filestartid++;
                    aFile.extension = item.Extension;
                    aFile.objectID = bug.id;
                    aFile.pathname = $"init/{bug.id}/{item.Name}";
                    aFile.size = Convert.ToInt32(item.Length);
                    aFile.title = item.Name;
                    bug.AFiles.Add(aFile);
                }
            }
        }

        private string GetType(string typestr)
        {
            if (typestr.Contains("代码错误"))
            {
                return "codeerror";
            }

            if (typestr.Contains("界面优化"))
            {
                return "interface";
            }
            if (typestr.Contains("设计缺陷"))
            {
                return "designdefect";
            }
            if (typestr.Contains("配置相关"))
            {
                return "config";
            }
            if (typestr.Contains("安装部署"))
            {
                return "install";
            }
            if (typestr.Contains("安全相关"))
            {
                return "security";
            }
            if (typestr.Contains("性能问题"))
            {
                return "performance";
            }
            if (typestr.Contains("标准规范"))
            {
                return "standard";
            }
            if (typestr.Contains("测试脚本"))
            {
                return "automation";
            }
            return "others";
        }

        private string GetResolution(string resolution)
        {
            if (resolution.Contains("设计如此"))
            {
                return "bydesign";
            }

            if (resolution.Contains("重复Bug"))
            {
                return "duplicate";
            }
            if (resolution.Contains("外部原因"))
            {
                return "external";
            }
            if (resolution.Contains("无法重现"))
            {
                return "notrepro";
            }
            if (resolution.Contains("延期处理"))
            {
                return "postponed";
            }
            if (resolution.Contains("不予解决"))
            {
                return "willnotfix";
            }
            return "fixed";
        }
        private string GetStatus(string status)
        {
            if (status.Contains("已解决"))
            {
                return "resolved";
            }

            if (status.Contains("激活"))
            {
                return "active";
            }
            if (status.Contains("已关闭"))
            {
                return "closed";
            }
            return "closed";
        }

        private int GetConfirmed(string confirmed)
        {
            if (confirmed.Contains("已确认"))
            {
                return 1;
            }

            if (confirmed.Contains("未确认"))
            {
                return 0;
            }
            return 1;
        }

        private string GetAccount(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            if (name.Contains("罗群益"))
            {
                return "luoqy";
            }
            if (name.Contains("罗高勇"))
            {
                return "gaoyong";
            }
            if (name.Contains("Lilian"))
            {
                return "Lilian";
            }
            if (name.Contains("lixueman"))
            {
                return "lixueman";
            }
            if (name.Contains("fanxu"))
            {
                return "fanxu";
            }
            if (name.Contains("徐瑶"))
            {
                return "xuyao";
            }
            if (name.Contains("amy"))
            {
                return "Amy";
            }
            if (name.Contains("李丹"))
            {
                return "lidan";
            }
            if (name.Contains("南竹"))
            {
                return "zoe";
            }
            if (name.Contains("谢冬冬"))
            {
                return "jano";
            }
            if (name.Contains("段仪"))
            {
                return "duanyi";
            }
            if (name.Contains("施燕"))
            {
                return "Sarina";
            }
            if (name.Contains("王天宇"))
            {
                return "tina";
            }
            if (name.Contains("Jennifer"))
            {
                return "Jennifer";
            }
            if (name.Contains("祝鑫"))
            {
                return "cici_zhu";
            }
            if (name.Contains("tanglei"))
            {
                return "tanglei";
            }
            if (name.Contains("胡笑峰"))
            {
                return "huxiaofeng";
            }
            if (name.Contains("叶华"))
            {
                return "yehua";
            }
            if (name.Contains("许光丽"))
            {
                return "xugl";
            }
            if (name.Contains("梁锦友"))
            {
                return "ljy";
            }
            if (name.Contains("杨柳"))
            {
                return "yangliu";
            }
            if (name.Contains("Anicka"))
            {
                return "Anicka";
            }
            if (name.Contains("赵广永"))
            {
                return "beck";
            }
            if (name.Contains("陈攀"))
            {
                return "chenpan";
            }
            return "admin";
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (log.Length > 0 && oldLogLength != log.Length)
            {
                int startindex = 0;
                int length = log.Length;
                if (log.Length > 32767)
                {
                    startindex = log.Length - 32767;
                    length = 32767;
                }
                textBox1.Text = log.ToString(startindex, length);
                oldLogLength = log.Length;
            }
        }
    }
}
