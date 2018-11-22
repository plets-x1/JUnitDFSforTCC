using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lesse.Modeling.Graph;
using Lesse.Modeling.TestPlanStructure;

namespace Lesse.JUnit.DFSforTCC {
    public class GenerateTestPlanDFSforTCC {
        #region Attributes
        //public List<CsvParamFile> paramFiles { get; set; }
        private int currLine = 0;
        private int maxLine = 0;
        private Boolean doAgain = false;
        private Regex param = new Regex (@"(?<param>{(?<file>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*).(?<column>[ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\sa-zA-Z0-9_!#$%&'+\/=?^`{|}~-]*)})", RegexOptions.IgnoreCase);
        //private List<CsvParamFile> usedFiles;
        private Boolean readFile;
        #endregion

        #region Public Methods
        public TestPlanForTCC PopulateTestPlan (List<String[]> sequence, DirectedGraph dg) {
            //this.paramFiles = paramFiles;
            TestPlanForTCC testPlan = new TestPlanForTCC ();
            PopulateTestCase (sequence, dg, testPlan);

            return testPlan;
        }
        #endregion

        #region Private Methods
        private void PopulateTestCase (List<String[]> sequence, DirectedGraph dg, TestPlanForTCC testPlan) {
            for (int k = 0; k < sequence.Count (); k++) {
                Edge edge = new Edge ();
                List<Edge> edges = new List<Edge> ();
                String[] arraySequence = sequence[k];
                int maxUseCaseLines = int.MaxValue;
                foreach (String input in arraySequence) {
                    edge = GetEdge (input, dg, edge.NodeB);
                    if (edge != null) {
                        edges.Add (edge);

                        foreach (KeyValuePair<String, String> pair in edge.Values) {
                            //int aux = GetUsedFilesLineCount(pair.Value);
                            //if (maxUseCaseLines > aux)
                            {
                                //maxUseCaseLines = aux;
                            }
                        }
                    }
                }

                TestCaseForTCC testCase = null;

                do {
                    testCase = FillTestCase (dg, edges, testCase);
                    if (testCase != null) {
                        testPlan.TestCases.Add (testCase);
                    }
                    //currLine++;
                } while (doAgain /*&& (currLine < maxLine)*/ );
            }
        }

        private TestCaseForTCC FillTestCase (DirectedGraph dg, List<Edge> edges, TestCaseForTCC testCase) {
            TestStepForTCC testStep;
            int index = 1;
            testCase = new TestCaseForTCC ("TestCase" + index);
            //testCase.Title += "_" + TestCaseForTCC.contWorkItemId;
            //testCase.WorkItemId = TestCase.contWorkItemId;
            //testCase.WriteFirstLine = TestCaseTags(dg, testCase);
            //TestCase.contWorkItemId++;
            testStep = new TestStepForTCC ();
            //usedFiles = new List<CsvParamFile>();
            foreach (Edge edge in edges) {
                readFile = false;
                Boolean isCycle = false;
                Boolean lastCycleTrans = false;
                if (edge.GetTaggedValue ("TDlastCycleTrans") != null) {
                    lastCycleTrans = (edge.GetTaggedValue ("TDlastCycleTrans").Equals ("true") ? true : false);
                }
                if (edge.GetTaggedValue ("TDcycleTran") != null) {
                    isCycle = true;
                }
                if (lastCycleTrans) {
                    //usedFiles = usedFiles.Distinct().ToList();
                    //foreach (CsvParamFile csv in usedFiles)
                    {
                        //csv.NextLine();
                    }
                    //usedFiles.Clear();
                }

                if (isCycle) {
                    //testStep = new TestStepForTCC();
                    //testStep.Index = index.ToString();
                    //testStep.Description = GenerateDescription(edge);
                    //testStep.ExpectedResult = GenerateExpectedResult(edge);
                    //testCase.TestSteps.Add(testStep);
                    //index++;
                } else {
                    testStep = new TestStepForTCC ();
                    testStep.ActionType = edge.GetTaggedValue ("ACTIONTYPE");
                    if (!String.IsNullOrEmpty (edge.GetTaggedValue ("PARAMS"))) {
                        testStep.Input = edge.GetTaggedValue ("PARAMS");
                    } else {
                        testStep.Input = "";
                    }
                    //testStep.IsAbstract = Boolean.Parse(edge.GetTaggedValue("METHODABSTRACT"));
                    testStep.Method = edge.GetTaggedValue ("METHOD");
                    if (!String.IsNullOrEmpty (edge.GetTaggedValue ("EXPECTEDRESULTS"))) {
                        testStep.Output = edge.GetTaggedValue ("EXPECTEDRESULTS");
                    } else {
                        testStep.Output = "";
                    }
                    testStep.Receiver = edge.NodeB.Name;
                    testStep.Return = edge.GetTaggedValue ("METHODRETURN");
                    testStep.Sender = edge.NodeA.Name;
                    //testStep.Visibility = edge.GetTaggedValue("METHODVISIBILITY");
                    String aux = edge.GetTaggedValue ("METHODPARAMQUANTITY");

                    if (String.IsNullOrEmpty (aux)) {
                        aux = "0";
                    }
                    for (int i = 0; i < int.Parse (aux); i++) {

                    }
                    //testStep.Index = index.ToString();
                    //testStep.Description = GenerateDescription(edge);
                    //testStep.ExpectedResult = GenerateExpectedResult(edge);
                    testCase.TestSteps.Add (testStep);
                    index++;
                    if (readFile) {
                        doAgain = true;
                    }
                }
            }
            return testCase;
        }

        private Edge GetEdge (String input, DirectedGraph dg, Node nodeB) {
            List<Edge> edges = dg.Edges.Where (x => x.GetTaggedValue ("INDEX").Equals (input)).ToList ();
            foreach (Edge edge in edges) {
                if (!edge.isChecked ()) {
                    if (nodeB == null) {
                        edge.Check ();
                        return edge;
                    }

                    if (edge.NodeA.Equals (nodeB)) {
                        edge.Check ();
                        return edge;
                    }
                }
            }
            return null;
        }

        #endregion
    }
}