using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;

public class KNNThreaded : MonoBehaviour
{


    //threads

    int threadOutput;
    Thread firstThread = null;
    KNNThread knnInstance = null;

    public int KNNAlgorithm(Vector3 currPosF0, Vector3 currPosF1, Vector3 currPosF2, Vector3 currPosF3, Vector3 currPosF4, List<List<Vector3>> PointList, List<List<Vector3>> ChillList, List<List<Vector3>> FistList, List<List<Vector3>> OkayList, int output)
    {
        if (firstThread != null && firstThread.IsAlive)
        {
            return -1;
        }
        else if (firstThread != null)
        {
            output = knnInstance.threadOutput;
            //Debug.Log("output right now is : " + output);
        }



        //-----------------------------------------------------------------------------------------------
        //Threading
        knnInstance = new KNNThread(currPosF0, currPosF1, currPosF2, currPosF3, currPosF4, PointList, ChillList, FistList, OkayList);

        firstThread = new Thread(new ThreadStart(knnInstance.doKNNWork));
        firstThread.Start();

        Debug.Log("Thread started");

        //output = threadOutput;
        //Debug.Log("output = threadoutput " + output);
        return output;
    }




    private class KNNThread
    {
        CompareVector2 compareV2 = new CompareVector2();

        Vector3 m_currPosF0;
        Vector3 m_currPosF1;
        Vector3 m_currPosF2;
        Vector3 m_currPosF3;
        Vector3 m_currPosF4;
        //why not use List<Vector3[]> ?
        List<List<Vector3>> m_PointList;
        List<List<Vector3>> m_ChillList;
        List<List<Vector3>> m_FistList;
        List<List<Vector3>> m_OkayList;


        List<Vector2> shortestDistList = new List<Vector2>();

        float shortestDist;

        float tmp0;
        float tmp1;
        float tmp2;
        float tmp3;
        float tmp4;

        //for counting
        int KNNPoint = 0;
        int KNNChill = 0;
        int KNNFist = 0;
        int KNNOkay = 0;


        int KNNk = 0;

        public int threadOutput;

        public KNNThread(Vector3 currPosF0, Vector3 currPosF1, Vector3 currPosF2, Vector3 currPosF3, Vector3 currPosF4, List<List<Vector3>> PointList, List<List<Vector3>> ChillList, List<List<Vector3>> FistList, List<List<Vector3>> OkayList)
        {
            m_currPosF0 = currPosF0;
            m_currPosF1 = currPosF1;
            m_currPosF2 = currPosF2;
            m_currPosF3 = currPosF3;
            m_currPosF4 = currPosF4;

            m_PointList = PointList;
            m_ChillList = ChillList;
            m_FistList = FistList;
            m_OkayList = OkayList;
        }

        public void doKNNWork()
        {
            // fill list with distances
            fillListWork(m_PointList, 0);
            fillListWork(m_ChillList, 1);
            fillListWork(m_FistList, 2);
            fillListWork(m_OkayList, 3);



            // need for sorting

            //Vector2 tmp = new Vector2(0, 0);
            //for (int i = 0; i < shortestDistList.Count; i++)
            //{
            //    for (int j = 0; j < shortestDistList.Count - 1; j++)
            //    {
            //        if (shortestDistList[j].x > shortestDistList[j + 1].x)
            //        {
            //            tmp = shortestDistList[j + 1];
            //            shortestDistList[j + 1] = shortestDistList[j];
            //            shortestDistList[j] = tmp;
            //        }
            //    }
            //}

            shortestDistList.Sort(compareV2);

           // Debug.Log("Initiating recognition :");

            for (int i = 0; i < shortestDistList.Count; i++)
            {
                if (shortestDistList[i].y == 0) { KNNPoint++; } //Point identifier = 0
                if (shortestDistList[i].y == 1) { KNNChill++; } //Chill identifier = 1 
                if (shortestDistList[i].y == 2) { KNNFist++; }  //Fist identifier  = 2
                if (shortestDistList[i].y == 3) { KNNOkay++; }  //Okay identifier  = 3

                KNNk = KNNk + 1;

                if (KNNk >= 25)
                {
                   

                    threadOutput = -1;

                    if ((KNNPoint / KNNk) > (KNNChill / KNNk) && (KNNPoint / KNNk) > (KNNFist / KNNk) && (KNNPoint / KNNk) > (KNNOkay / KNNk))
                    {
                        threadOutput = 0;
                        Debug.Log("KNNRes POINT");
                       

                        break;
                    }
                    if ((KNNChill / KNNk) > (KNNPoint / KNNk) && (KNNChill / KNNk) > (KNNFist / KNNk) && (KNNChill / KNNk) > (KNNOkay / KNNk))
                    {
                        threadOutput = 1;

                        Debug.Log("KNNRes CHILL");

                        break;
                    }

                    if ((KNNFist / KNNk) > (KNNPoint / KNNk) && (KNNFist / KNNk) > (KNNChill / KNNk) && (KNNFist / KNNk) > (KNNOkay / KNNk))
                    {
                        threadOutput = 2;

                        Debug.Log("KNNRes Fist");
                       

                        break;
                    }

                    if ((KNNOkay / KNNk) > (KNNFist / KNNk) && (KNNOkay / KNNk) > (KNNChill / KNNk) && (KNNOkay / KNNk) > (KNNFist / KNNk))
                    {
                        threadOutput = 3;

                        Debug.Log("KNNRes Okay");
                       
                        break;
                    }
                }

            }

            KNNPoint = 0;
            KNNChill = 0;
            KNNFist = 0;
            KNNOkay = 0;
            KNNk = 0;
            shortestDistList.Clear();


            Debug.Log("Thread done");
            return;

        }

        // computes the average distance of current position to every List of Vector3s and adds it in a list with given identifier
        public void fillListWork(List<List<Vector3>> m_List, int identifier)
        {
            List<float> tmp0List = new List<float>();
            List<float> tmp1List = new List<float>();
            List<float> tmp2List = new List<float>();
            List<float> tmp3List = new List<float>();
            List<float> tmp4List = new List<float>();

            // Point List stuff
            foreach (Vector3 v in m_List[0])
            {
                //shortestDist = 1.0f;
                tmp0 = EuclidDist(m_currPosF0, v);
                tmp0List.Add(tmp0);

            }

            foreach (Vector3 v in m_List[1])
            {
                //shortestDist = 1.0f;
                tmp1 = EuclidDist(m_currPosF1, v);
                tmp1List.Add(tmp1);

            }

            foreach (Vector3 v in m_List[2])
            {
                //shortestDist = 1.0f;
                tmp2 = EuclidDist(m_currPosF2, v);
                tmp2List.Add(tmp2);

            }

            foreach (Vector3 v in m_List[3])
            {
                //shortestDist = 1.0f;
                tmp3 = EuclidDist(m_currPosF3, v);
                tmp3List.Add(tmp3);

            }

            foreach (Vector3 v in m_List[4])
            {
                //shortestDist = 1.0f;
                tmp4 = EuclidDist(m_currPosF4, v);
                tmp4List.Add(tmp4);

            }

            //shortestDist = (tmp0List[0] + tmp1List[0] + tmp2List[0] + tmp3List[0] + tmp4List[0]) / 5;
            //shortestDistList.Add(new Vector2 (shortestDist, 0));

            //why is this not 0?
            shortestDist = 1;
            // 0 Point identifier

            for (int i = 0; i < tmp0List.Count; i++)
            {
                float tmpRes;
                tmpRes = (tmp0List[i] + tmp1List[i] + tmp2List[i] + tmp3List[i] + tmp4List[i]) / 5;

                //why is this commented out?
                //if (tmpRes < shortestDist)
                //{
                shortestDist = tmpRes;
                shortestDistList.Add(new Vector2(shortestDist, identifier));
                // 0 Point identifier
                //}

            }



        }

        // why not use Vector3.Distance() instead?
        private float EuclidDist(Vector3 currPos, Vector3 ListPos)
        {
            float output;


            output = Mathf.Sqrt((currPos.x - ListPos.x) * (currPos.x - ListPos.x)
                              + (currPos.y - ListPos.y) * (currPos.y - ListPos.y)
                              + (currPos.z - ListPos.z) * (currPos.z - ListPos.z));
            return output;
        }
    }

    public class CompareVector2 : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {

            return x.x.CompareTo(y.x);
        }
    }
}


