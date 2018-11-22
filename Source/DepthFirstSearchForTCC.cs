using System;
using System.Collections.Generic;
using Lesse.Core.ControlAndConversionStructures;
using Lesse.Core.ControlStructure;
using Lesse.Core.Interfaces;
using Lesse.Modeling.Graph;
using Lesse.Modeling.TestPlanStructure;

namespace Lesse.JUnit.DFSforTCC {
    public class DepthFirstSearchForTCC : SequenceGenerator {
        #region Attributes
        private DirectedGraph dg;
        //List<TestStep> testStepsList = new List<TestStep>();
        //List<TestCase> testCasesList = new List<TestCase>();
        private List<Edge> Edges;
        private List<Node> Nodes;
        private List<Node> Finals;
        private Stack<String> Pilha;
        private TestPlanForTCC testPlan;
        #endregion

        #region Constructor
        public DepthFirstSearchForTCC () {
            //this.testPlan = new TestPlan();
        }
        #endregion

        #region Public Methods
        public override List<GeneralUseStructure> GenerateSequence (List<GeneralUseStructure> listGeneralStructure, ref int tcCount, StructureType type) {
            GenerateTestPlanDFSforTCC populate = new GenerateTestPlanDFSforTCC ();
            List<TestPlanForTCC> listPlan = new List<TestPlanForTCC> ();
            List<GeneralUseStructure> listScript = new List<GeneralUseStructure> ();
            List<GeneralUseStructure> listSequenceGenerationStructure = base.ConvertStructure (listGeneralStructure, type);
            //paramFiles = listGeneralStructure.OfType<CsvParamFile>().ToList();

            foreach (GeneralUseStructure sgs in listSequenceGenerationStructure) {
                this.dg = (DirectedGraph) sgs;
                List<String[]> sequence = this.GenerateTestCases ();
                testPlan = populate.PopulateTestPlan (sequence, dg);
                //tcCount += testPlan.TestCases.Count;
                listPlan.Add (testPlan);
            }

            //GeneralTPGenerator(listPlan);

            foreach (TestPlanForTCC testPlan in listPlan) {
                GeneralUseStructure sc = (GeneralUseStructure) testPlan;
                listScript.Add (sc);
            }
            //TestCase.contWorkItemId = 1000;
            return listScript;
        }

        public List<String[]> GenerateTestCases () {
            this.testPlan = new TestPlanForTCC ();
            this.testPlan.Name = dg.Name;

            List<String[]> testCases = new List<String[]> ();
            List<String[]> sequence = new List<String[]> ();

            sequence = GetAllFinalSequences (this.dg.RootNode, true);

            //List<String[]> filteredSequences = FilterSequences(sequence);
            //testCases.AddRange(filteredSequences);
            testCases.AddRange (sequence);

            return testCases;
        }
        #endregion

        #region Private Methods
        private List<String[]> GetAllFinalSequences (Node node, Boolean avoidCycles) {
            List<String[]> ret = new List<String[]> ();
            Pilha = new Stack<String> ();
            List<String> DFSSeqs = DFS (node);

            String[] delimiter = new String[] { ";" };
            String[] sequence;
            foreach (String seq in DFSSeqs) {
                sequence = seq.Split (delimiter, StringSplitOptions.RemoveEmptyEntries);
                Array.Reverse (sequence);
                ret.Add (sequence);
            }
            return ret;
        }

        private List<String> DFS (Node v) {
            this.Nodes = dg.Nodes;
            this.Edges = dg.Edges;
            this.Finals = dg.Finals;

            List<Edge> vEdges;
            List<String> ret = new List<String> ();

            vEdges = GetAllEdgesFrom (v);
            Boolean auxiliar = AreAllChecked (this.Edges);
            Boolean returning = false;

            if (vEdges.Count == 0 || auxiliar) {
                ret.Add (PrintStack (Pilha));
            }

            //if (Finals.Contains(v))
            //{
            //ret.Add(PrintStack(Pilha));
            //}

            if (v != null) {
                vEdges = GetAllEdgesFrom (v);
                foreach (Edge edge in vEdges) {
                    if (!returning) {
                        Node w = edge.NodeB;
                        if (!(edge.isChecked ()) && !auxiliar) {
                            edge.Check ();
                            if (!w.isChecked ()) {
                                Pilha.Push (edge.GetTaggedValue ("Index"));
                                returning = true;
                                ret.AddRange (DFS (w));
                                Pilha.Pop ();
                            }
                            edge.UnCheck ();
                        }
                    }
                }
            }

            return ret;
        }

        private Boolean AreAllChecked (List<Edge> edges) {
            foreach (Edge edge in edges) {
                if (!edge.isChecked ()) {
                    return false;
                }
            }
            return true;
        }
        private List<Edge> GetAllEdgesFrom (Node v) {
            List<Edge> ret = new List<Edge> ();
            foreach (Edge edge in Edges) {
                if (edge.NodeA.Equals (v)) {
                    ret.Add (edge);
                }
            }
            return ret;
        }

        private String PrintStack (Stack<String> pilha) {
            String ret = "";
            foreach (String s in Pilha) {
                ret += s + ";";
            }
            ret = ret.Substring (0, ret.Length - 1);

            return ret;
        }

        private void GeneralTPGenerator (List<TestPlan> listPlan) {
            TestPlan testPlanGeral = new TestPlan ();
            TestCase testCaseGeral = new TestCase ("GeneralTestCase");
            TestPlan testPlanAux = listPlan[listPlan.Count - 1];
            TestCase testAux = testPlanAux.TestCases[testPlanAux.TestCases.Count - 1];
            List<TestStep> listTestStep = new List<TestStep> ();

            int valor = testAux.WorkItemId;
            valor = valor + 1;
            testCaseGeral.WorkItemId = valor;
            testCaseGeral.Title = testCaseGeral + "_" + valor;
            foreach (TestPlan testPlan in listPlan) {
                foreach (TestCase testCase in testPlan.TestCases) {
                    bool initial = true;
                    foreach (TestStep testStep in testCase.TestSteps) {
                        if (initial) {
                            testStep.Title = testCase.Title;
                            initial = false;
                        }
                        listTestStep.Add (testStep);
                    }
                }
            }
            testCaseGeral.TestSteps.AddRange (listTestStep);
            testPlanGeral.TestCases.Add (testCaseGeral);
            listPlan.Add (testPlanGeral);
        }

        private List<String[]> FilterSequences (List<String[]> sequences) {
            List<String[]> filteredSequences = new List<String[]> ();

            foreach (String[] sequence in sequences) {
                List<String> filteredSequence = new List<String> ();
                for (int i = 0; i < sequence.Length; i++) {
                    Double stepIndex = Double.Parse (sequence[i]);
                    if (!i.Equals (sequence.Length - 1)) {
                        Double nextStepIndex = Double.Parse (sequence[i + 1]);
                        filteredSequence.Add (sequence[i]);

                        if (stepIndex > nextStepIndex) {
                            //filteredSequences.Add(filteredSequence.ToArray());
                            break;
                        }
                    } else {
                        Double previousStepIndex = Double.Parse (sequence[i - 1]);

                        if (stepIndex > previousStepIndex) {
                            filteredSequence.Add (sequence[i]);
                        }
                    }
                }
                filteredSequences.Add (filteredSequence.ToArray ());
            }
            return filteredSequences;
        }
        #endregion

    }
}