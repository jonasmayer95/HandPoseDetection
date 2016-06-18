using UnityEngine;
using System.Collections;
using System.IO;

public class CSVParserTest : MonoBehaviour {

	// Use this for initialization
	public OutputHand outHandNew, outHandOld;
	public AngleBasedHandModel handNew, handOld;
	StreamReader read;
	int first;
	int rating;
	string inputFileName, outputFileName;
	void Start () {
		/*
		handOld = PostureDataHandler.instance.getSublist(TrainingUnit.Posture.idle)[Random.Range(0,PostureDataHandler.instance.getSublist(TrainingUnit.Posture.idle).Count)].hand;
		outHandOld.visualizeHand (handOld);

		handNew = new AngleBasedHandModel ();
		string HandString = handOld.ToCSVString (", ");
		string[] endl = { ", " };
		string[] input = HandString.Split (endl, System.StringSplitOptions.RemoveEmptyEntries);
		Debug.Log(handNew.parseCSV (input, 0));
		outHandNew.visualizeHand (handNew);
		string header = 
			"Name, Rating, Discomfort, Comfort, InterDis, AbductionDis, HyperDis, Interindex, Intermiddle, Interring, Interpinky, Abductionindex, Abductionmiddle, Abductionring, Abductionpinky," +
			" Hyperindex, Hypermiddle, Hyperring, Hyperpinky, RRPindex, RRPmiddle, RRPring, RRPpinky, RRPThumb, RandomHandThumbIP, RandomHandThumbMP, RandomHandThumbTMCx, RandomHandThumbTMCy, RandomHandThumbTMCz," +
			" RandomHandThumbTMCw, RandomHandindexDIP, RandomHandindexPIP, RandomHandindexMCPx, RandomHandindexMCPy, RandomHandindexMCPz, RandomHandindexMCPw, RandomHandmiddleDIP, RandomHandmiddlePIP, RandomHandmiddleMCPx," +
			" RandomHandmiddleMCPy, RandomHandmiddleMCPz, RandomHandmiddleMCPw, RandomHandringDIP, RandomHandringPIP, RandomHandringMCPx, RandomHandringMCPy, RandomHandringMCPz, RandomHandringMCPw, RandomHandpinkyDIP," +
			" RandomHandpinkyPIP, RandomHandpinkyMCPx, RandomHandpinkyMCPy, RandomHandpinkyMCPz, RandomHandpinkyMCPw, RandomHandRotx, RandomHandRoty, RandomHandRotz, RandomHandRotw, RandomHandPosx, RandomHandPosy," +
			" RandomHandPosz";	

		inputFileName =PostureDataHandler.instance.filePath + "ComfortEvaluationData"+UserStudyData.instance.fileEnding;

		read = new StreamReader(inputFileName);
		string oldHeader = read.ReadLine();
		first = System.Array.IndexOf (oldHeader.Split (endl, System.StringSplitOptions.RemoveEmptyEntries), "RandomHandThumbIP");
		Debug.Log (first);
		*/
		convertAll ();
	}


	public void test()
	{
		/*
		if (read != null && !read.EndOfStream) {
			string dataLine = read.ReadLine ();
			string[] endl = { ", " };
			string[] input = dataLine.Split (endl, System.StringSplitOptions.RemoveEmptyEntries);
			Debug.Log (handNew.parseCSV (input, first));
			outHandNew.visualizeHand (handNew);
		} else
			read.Dispose ();
			*/
	}
	// Update is called once per frame
	void Update () {
	
	}

	public void convertAll()
	{
		inputFileName = PostureDataHandler.instance.filePath + "ComfortEvaluationData"+UserStudyData.instance.fileEnding;
		outputFileName = PostureDataHandler.instance.filePath + "ComfortEvaluationCompressed"+UserStudyData.instance.fileEnding;
		read = new StreamReader(inputFileName);
		string oldHeader = read.ReadLine();
		string[] endlArr = { ", " };
		first = System.Array.IndexOf (oldHeader.Split (endlArr, System.StringSplitOptions.RemoveEmptyEntries), "RandomHandThumbIP");
		rating = System.Array.IndexOf (oldHeader.Split (endlArr, System.StringSplitOptions.RemoveEmptyEntries), "Rating");
		int currentRating;

		if (File.Exists (outputFileName))
			File.Delete (outputFileName);

		string endl = ", ";
		string fileHeader = 
			"Rating" + endl +
			Discomfort.getInterFingerCSVHeader (endl) +
			Discomfort.getAbductionCSVHeader (endl) +
			Discomfort.getHyperExtensionCSVHeader (endl) +
			Comfort.getRRPCSVHeader (endl);

		File.AppendAllText(outputFileName,fileHeader+System.Environment.NewLine);


		while (!read.EndOfStream) {
			string dataLine = read.ReadLine ();
			string[] input = dataLine.Split (endlArr, System.StringSplitOptions.RemoveEmptyEntries);
			currentRating = int.Parse (input[rating]);
			handNew.parseCSV (input, first);

			File.AppendAllText(
				outputFileName, 
				currentRating + endl +
				Discomfort.getInterFingerCSV(handNew, endl) +
				Discomfort.getAbductionCSV(handNew, endl) +
				Discomfort.getHyperExtensionCSV(handNew, endl) +
				Comfort.getRRPCSV(handNew, endl) +
				System.Environment.NewLine
			);


		}
		Debug.Log ("Done!");
		read.Close ();
	}
}
