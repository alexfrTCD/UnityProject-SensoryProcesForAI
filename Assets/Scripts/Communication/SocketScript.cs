using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketScript
{
    Thread threat;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25003;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;

    Vector3 receivedPosition = Vector3.zero;
    Vector3 objectPosition = Vector3.zero;
    string receivedAction = "init";
    bool objectWasEaten = false;
    bool running;
    bool initReceived = false;
    int maxBytes = 1024;
    NetworkStream networkStream;
    int numberInstructionsPython = 1;
    // Start is called before the first frame update
    public void initCommunication(WorldClass board, Vector3 avatarPosition, Vector3 avatarForward)
    {
        //ThreadStart ts = new ThreadStart(GetInfo);
        Thread threat = new Thread(() => GetInfo(board, avatarPosition, avatarForward));
        threat.Start();

    }

    public void GetInfo(WorldClass board, Vector3 avatarPosition, Vector3 avatarForward)
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();
        client.ReceiveBufferSize = maxBytes;
        running = true;


        networkStream = client.GetStream();
        while (running)
        {
            if(networkStream != null)
            {
                if (!initReceived)
                {
                    initGame(avatarPosition, avatarForward);
                    initPossiblePositions(board);
                    initReceived = true;
                }

                else
                {
                    if(receivedAction != "notAchieved")
                    {
                        actualizePosition();
                    }
                }
                
            }

            else
            {
                initReceived = false;
            }

        }
        listener.Stop();

    }

    private string receiveInfo(string infoType)
    {
        //Char option == 1 byte size
        int size = 1;
        if (infoType == "Vector3")
        {
            //The size for a Vector3 will be 50 always 16,16,16
            size = 50;
        }
        else if (infoType == "number")
        {
            //The size for a single number will be 16
            size = 16;
        }
        else if (infoType == "action")
        {
            //The max size for an action = 10
            size = 16;
        }

        byte[] buffer = new byte[size];
        //Receiving Data from the Host
        int bytesRead = networkStream.Read(buffer, 0, size);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);


    }
    private void sendInfo(string info, string infoType)
    {
        if (infoType == "number")
        {
            //We limit each number to 16 bytes
            maxBytes = 16;
            if (Encoding.ASCII.GetBytes(info).Length < maxBytes)
            {

                int difference = maxBytes - info.Length;

                //Here we are doing a loop that made us lose some time. Change it later
                for (int i = 0; i < difference; i++)
                {
                    info = info + ' ';
                }
            }
        }

        else if (infoType == "message")
        {
            //We limit each message to 16 bytes
            maxBytes = 16;
            if (Encoding.ASCII.GetBytes(info).Length < maxBytes)
            {

                int difference = maxBytes - info.Length;

                //Here we are doing a loop that made us lose some time. Change it later
                for (int i = 0; i < difference; i++)
                {
                    info = info + "-";
                }
            }
        }

        else if (infoType == "name")
        {
            //We limit each message to 16 bytes
            maxBytes = 16;
            if (Encoding.ASCII.GetBytes(info).Length < maxBytes)
            {

                int difference = maxBytes - info.Length;
                //Debug.Log("name difference = " + difference.ToString());
                //Here we are doing a loop that made us lose some time. Change it later
                for (int i = 0; i < difference; i++)
                {
                    info = info + "-";
                }
            }
            //Debug.Log("Final name = " + info);
        }
        //Right now, another message is a char, only one byte. No changes, no more possibilites



        networkStream.Write(Encoding.ASCII.GetBytes(info), 0, Encoding.ASCII.GetBytes(info).Length);
    }
    private void initGame(Vector3 avatarPosition, Vector3 avatarForward)
    {

        sendNewPosition(avatarPosition);
        if(avatarForward == new Vector3(1, 0, 0))
        {
            Debug.Log("rotation X");
            sendNewValue(1);
            sendNewValue(0);
        }
        else if (avatarForward == new Vector3(-1, 0, 0))
        {
            Debug.Log("rotation -X");
            sendNewValue(-1);
            sendNewValue(0);
        }
        else if (avatarForward == new Vector3(0, 0, 1))
        {
            Debug.Log("rotation Z");
            sendNewValue(0);
            sendNewValue(1);
        }
        else if (avatarForward == new Vector3(0, 0, -1))
        {
            Debug.Log("rotation -Z");
            sendNewValue(0);
            sendNewValue(-1);
        }
        else
        {
            Debug.Log("rotation??");
            sendNewValue(0);
            sendNewValue(0);
        }


    }

    private void initPossiblePositions(WorldClass board)
    {
        sendInfo(board.getRows().ToString(), "number");
        sendInfo(board.getColumns().ToString(), "number");
        //sendInfo("Init Pos", "string", network);
        sendInfo(board.getPossiblePositions().Count.ToString(), "number");
        //Debug.Log("numberPossiblePositions = " + board.getPossiblePositions().Count);
        for (int position = 0; position < board.getPossiblePositions().Count; position++)
        {
            
            sendInfo(board.getPossiblePositions()[position].x.ToString(), "number");
            sendInfo(board.getPossiblePositions()[position].z.ToString(), "number");
            Debug.Log("Possible position: " + board.getPossiblePositions()[position].x.ToString()
                + board.getPossiblePositions()[position].z.ToString());
        }
    }

    public void actualizePossiblePositions(WorldClass board)
    {
        sendInfo(board.getPossiblePositions().Count.ToString(), "number");
        //Debug.Log("numberPossiblePositions = " + board.getPossiblePositions().Count);
        for (int position = 0; position < board.getPossiblePositions().Count; position++)
        {

            sendInfo(board.getPossiblePositions()[position].x.ToString(), "number");
            sendInfo(board.getPossiblePositions()[position].z.ToString(), "number");
            //Debug.Log("Possible position: " + board.getPossiblePositions()[position].x.ToString()
            //    + board.getPossiblePositions()[position].z.ToString());
        }
    }


    private void actualizePosition()
    {
        string action = receiveInfo("action");
        //Debug.Log("Inside actualize position, action received: " + action);
        //Debug.Log("Actualizing position with action: "+  action);
        //Debug.Log("Actualize position action: " + action);
        if (action != null && action !="")
        {
            receivedAction = "";
            bool founded = false;
            int count = 0;
            while (!founded)
            {
                //Debug.Log("action: " + action);
                //Debug.Log("Inside while ACTION:" + action[count].ToString());
                if (action[count] == '0')
                {
                    founded = true;
                }
                else
                {
                    receivedAction = receivedAction + action[count];
                    count++;
                }
            }
            //Debug.Log("received action modified: " + receivedAction);
            numberInstructionsPython++;
        }
    }

    public float receiveNumber()
    {
        string numberReceived = receiveInfo("number");
        //Debug.Log("Number received: " + numberReceived);
        return float.Parse(numberReceived);
    }


    private Vector3 StringToVector3(string dataReceived)
    {
        //Debug.Log("Dataprocessing : " + dataReceived);

        //split the item
        string[] sArray = dataReceived.Split(',');


        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse("-1"),
            float.Parse(sArray[2]));
        //Debug.Log("string to vector final conversion: " + result.x.ToString() + "," + result.y.ToString() + "," + result.z.ToString());
        return result;


    }

    public Vector3 getNewPosition()
    {
        return receivedPosition;
    }
    public Vector3 getObjectPosition()
    {
        return objectPosition;
    }
    public string getAction()
    {
        return receivedAction;
    }
    public void setAction(string newAction)
    {
        receivedAction = newAction;
    }
    public bool getObjectWasEaten()
    {
        return objectWasEaten;
    }
    public void setObjectWasEaten(bool boolean)
    {
        objectWasEaten = boolean;
    }

    public void sendNewPosition(Vector3 newPosition)
    {
        //Debug.Log("checking new pos");
        //Debug.Log(newPosition.x.ToString() + newPosition.y.ToString() + newPosition.z.ToString());
        sendInfo(newPosition.x.ToString(), "number");
        sendInfo(newPosition.y.ToString(), "number");
        sendInfo(newPosition.z.ToString(), "number");
    }

    public void sendNewValue(float value)
    {
        sendInfo(value.ToString(), "number");
    }
    public void sendNewList(List<ObjectClass> listToSend, string sendListName)
    {
        //Debug.Log("Send new List");
        if(sendListName == "sight")
        {
            sendInfo(0.ToString(), "number");
        }
        else if(sendListName  == "smell")
        {
            sendInfo(1.ToString(), "number");
        }
        else if (sendListName == "hearing")
        {
            sendInfo(2.ToString(), "number");
        }
        else if (sendListName == "taste")
        {
            sendInfo(3.ToString(), "number");
        }
        else
        {
            //touch
            sendInfo(4.ToString(), "number");
        }
        //Debug.Log("Sense: " + sendListName);
        //foreach (GameObject go in listToSend)
        //{
        //    Debug.Log("Position object " + go.gameObject.name + " = " + go.transform.position.x.ToString() + go.transform.position.z.ToString());
        //}

        //Debug.Log("List name " + sendListName);
        sendInfo(listToSend.Count.ToString(), "number");


        //Debug.Log("Length " + listToSend.Count.ToString());
        //Debug.Log("Info sended newList: " + listToSend.Count.ToString());
        for (int i = 0; i < listToSend.Count; i++)
        {
            if (listToSend[i] != null)
            {
                //Debug.Log("Character sended = " + listToSend[i].GetComponent<ObjectClass>().Character);
                //Debug.Log("Position object " + listToSend[i].gameObject.name + " = " + listToSend[i].transform.position.x.ToString() + listToSend[i].transform.position.y.ToString() + listToSend[i].transform.position.z.ToString());

                sendObject(listToSend[i].gameObject);

            }
        }

    }

    private void sendObject(GameObject gameObject)
    {
        sendInfo(gameObject.GetComponent<ObjectClass>().getObjectType(), "name");
        //Sending object position
        sendInfo(gameObject.transform.position.x.ToString(), "number");
        sendInfo(gameObject.transform.position.y.ToString(), "number");
        sendInfo(gameObject.transform.position.z.ToString(), "number");

        //Sending distance to the object
        sendInfo(gameObject.GetComponent<ObjectClass>().getDistance().ToString(), "number");

    }

    public bool getInit()
    {
        return initReceived;
    }

    public int getNumberInstructions()
    {
        return numberInstructionsPython;
    }

}
