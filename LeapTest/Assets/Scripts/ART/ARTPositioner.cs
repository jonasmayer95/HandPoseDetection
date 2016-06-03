using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
public class ARTPositioner : MonoBehaviour {

    public Transform target;

    public int markerID = 1;

    public sVector3 offset = new sVector3(0,-0.3f,0);


    public const int BufferSize = 8192; //up this to theoretical UDP limit if required.
    private readonly string[] frameDelimiters = { "\r\n" };
    private readonly string[] bodyDelimiters = { " ", "]", "[" };
    public volatile bool terminate = false;
    public bool timedOut = false, isVisible = false;

    Vector3 position = new Vector3();
    Quaternion rotation = Quaternion.identity;

    [HideInInspector]
    public byte[] buffer = new byte[BufferSize];

    private Socket artSocket;
    private Thread recvThread;
    private EndPoint trackingEndpoint = new IPEndPoint(IPAddress.Parse("131.159.10.100"), 0);
    // Use this for initialization
    void Start()
    {
        InitSocket();
        InitRecvThread();
    }

    // Update is called once per frame
    void Update()
    {
        updateMarker();
    }


    private void InitRecvThread()
    {
        //set up thread, kick off recv loop
        if (recvThread == null)
        {
            recvThread = new Thread(() => Run());
            recvThread.Start();
            Debug.Log("ArtFilter: started recvThread");
        }
    }

    private void InitSocket()
    {
        artSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        artSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 5000));
        artSocket.ReceiveTimeout = 3000; //3sec timeout, then close the socket
    }

    private void Run()
    {
        try
        {
            while (!terminate)
            {
                int bytesRead = artSocket.ReceiveFrom(buffer, BufferSize, SocketFlags.None, ref trackingEndpoint);
            }

            artSocket.Close();
            Debug.Log("ArtFilter: closed artSocket, terminating regularly");
        }
        catch (SocketException ex)
        {
            if (artSocket != null && ex.SocketErrorCode == SocketError.TimedOut)
            {
                artSocket.Close();
                timedOut = true;
                Debug.Log("ArtFilter: closed artSocket due to timeout");
            }
        }
    }

    public bool isTracking()
    {
        return !(timedOut || terminate) && isVisible;
    }

    private void updateMarker()
    {
        string data;
        isVisible = false;

        lock (buffer)
        {
            //parse stuff, fill into our struct, return
            data = Encoding.ASCII.GetString(buffer);
        }

        var lines = data.Split(frameDelimiters, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            int lastPos = 0;

            //find first space
            int findPos = line.IndexOf(' ');

            //invalid line
            if (findPos == -1)
                break;


            string recordType = line.Substring(lastPos, findPos - lastPos);


            //the only useful record type for us
            if (recordType == "6d")
            {
                //get body count (first number after record type)
                lastPos = findPos + 1; //start one char after last space
                findPos = line.IndexOf(' ', lastPos);

                string trackID = line.Substring(lastPos, findPos - lastPos);
                int id = int.Parse(trackID);

                if(id == markerID)
                {
                    isVisible = true;
                    //do stuff for each 6d body
                    if (lastPos > line.Length)
                    {
                        Debug.Log("6d: Unexpected end of line");
                        break;
                    }

                    //find first '['
                    if ((lastPos = line.IndexOf('[', lastPos)) == -1)
                    {
                        //there is no body data
                        break;
                    }

                    //find record ending
                    findPos = line.IndexOf("] ", lastPos);

                    string record = line.Substring(lastPos, findPos - lastPos);

                    var positions = record.Split(bodyDelimiters, StringSplitOptions.RemoveEmptyEntries);

                    //parse positions and write them into our struct
                    //rotations seem to be in euler angles
                    //int id;
                    //double qual, px, py, pz, rx, ry, rz;
                    position = new Vector3(float.Parse(positions[2]), float.Parse(positions[4]), float.Parse(positions[3]));
                    rotation = Quaternion.Euler(-float.Parse(positions[5]), -float.Parse(positions[7]), -float.Parse(positions[6]));
                    position /= 1000f;
                    position += offset;

                    target.position = position;
                    target.rotation = rotation;
                    break;
                }
            }
        }
    }

    public void setTarget()
    {
        target.position = position;
        target.localRotation = rotation;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public Quaternion getRotation()
    {
        return rotation;
    }
    public Vector3 getForward()
    {
        return transform.forward;
    }
}

