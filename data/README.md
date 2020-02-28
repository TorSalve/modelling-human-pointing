# pointing dataset

## study design

We have collected data that describes the movement of some participants while pointing. For this purpose, we have built an application that can collect this exact data. We use VR to present the participant with a controlled environment, in which the user points at some targets in the space. 

The targets are arranged in a 3x3x3 grid, such that we have 27 different targets at fixed positions. The center rows of the grid is at a height of 1.49 m over the floor, since that is the average shoulder height of a human.

<img src="https://raw.githubusercontent.com/TorSalve/modelling-human-pointing/master/docs/_images/targets.png" width="500" title="Depiction of the targets task."/>

*Each target (green) labeled with its positon arranged in the 3x3x3 grid. The red cross is the position participants stand on. [m]*

The participant is asked to perform two different tasks; a calibration task and a collection task. In both tasks the participant holds a controller in the left hand and is wearing a marker setup. The controller has two buttons we will denote as A and B. The marker setup is able to being tracked using a motion capturing system. It consists of seven marker sets that are attached on the index finger, hand, forearm, upper arm, right shoulder, Head Mounted Device (HMD) and left shoulder.

In the collection task, the participant is asked to point at a shown target. They point at each target five times, for a total of 135 repetitions. Before each repetition, the participant is asked to put the arm in a predefined position. The participant now presses button A to start the repetition. This makes a target appear, which is to be pointed at. When the participant is confident that they point at the target, they press button B. This concludes one repetition.

The design of the task aims to promote natural pointing movement.

## the data

The data is scattered across four files:
* **participants.csv** - containing information about the participant.
* **calibrations.csv** - containing calibration data, produced by a participant.
* **collections.csv** - containing the body movement data for each repetition.
* **targets.csv** - containing the targets the participants pointed at.

### participants.csv

The participant ID (**pid**) is used throughout the different files. It describes which participant produced a pointing movement. All distance measures are in meters.

*.csv columns*
* **pid** - participant ID
* **handedness** - participant handedness
* **gender** - participant gender
* **age** - participant age
* **forearmLength** - length of the forearm
* **forearmMarkerDist** - distance from the forarm marker to the elbow
* **indexFingerLength** - index finger length
* **upperArmLength** - length of the upper arm
* **upperArmMarkerDist** - distance from the upper arm marker to the elbow
* **height** - participant height
* **rightShoulderMarkerDist.X** - horizontal distance from the right shoulder marker to the participants shoulder
* **rightShoulderMarkerDist.Y** - vertical distance from the right shoulder marker to the participants shoulder

### collections.csv

This file contains the most interesting part of the collected data. Each line in the file is a sample of the participants body position. Each repetition is logged with 50Hz, ie. a samples is created every 0.02 seconds. One sample consists of the positions and orientations of seven fix-points on the participants body: the index finger, hand, forearm, upper arm, right shoulder, HMD and left shoulder. A collection consists of a number of samples and is identified by the combination of participant ID (**pid**) and collection ID (**cid**). Note that only the combination is unique, while the participant ID is repeated across collections and the collection ID is repeated across participants. The number of samples in a collection varies based on the time a participant needed to conclude the repetition.

We define the notion of an endpoint as the last sample in each collection, ie. the body position, when a participant pressed the "stop"-trigger on the controller. In the *endpoints*-dataset only these samples are included, while the *full*-dataset contains all collected samples.

*.csv columns*
* **pid** - ID of the participant, that performed the calibration
* **cid** - collection ID / repetition number
* **time** - seconds since the beginning of the trial
* **indexFinger.X**, **indexFinger.Y**, **indexFinger.Z** - the position of the index finger
* **hand.X**, **hand.Y**, **hand.Z** - the position of the hand
* **forearm.X**, **forearm.Y**, **forearm.Z** - the position of the forearm
* **upperArm.X**, **upperArm.Y**, **upperArm.Z** - the position of the upper arm
* **rightShoulder.X**, **rightShoulder.Y**, **rightShoulder.Z** - the position of the right shoulder
* **hmd.X**, **hmd.Y**, **hmd.Z** - the position of the HMD
* **leftShoulder.X**, **leftShoulder.Y**, **leftShoulder.Z** - the position of the left shoulder
* **indexFingerO.X**, **indexFingerO.Y**, **indexFingerO.Z** - the orientation of the index finger
* **handO.X**, **handO.Y**, **handO.Z** - the orientation of the hand
* **forearmO.X**, **forearmO.Y**, **forearmO.Z** - the orientation of the forearm
* **upperArmO.X**, **upperArmO.Y**, **upperArmO.Z** - the orientation of the upper arm
* **rightShoulderO.X**, **rightShoulderO.Y**, **rightShoulderO.Z** - the orientation of the right shoulder
* **hmdO.X**, **hmdO.Y**, **hmdO.Z** - the orientation of the HMD
* **leftShoulderO.X**, **leftShoulderO.Y**, **leftShoulderO.Z** - the orientation of the left shoulder

### calibrations.csv

At the beginning of each experiment, the participants calibrates the VR application. Each participant completes three tasks, depicted in figure 1. Each line in the .csv-file contains a sample (similar to the *collections.csv*-file), where the calibration ID (**calid**) describes the calibration number (see figure 1) and the participant ID (**pid**) tells which participant performed the calibration.

![https://raw.githubusercontent.com/TorSalve/modelling-human-pointing/master/docs/_images/calibration_task.png](https://raw.githubusercontent.com/TorSalve/modelling-human-pointing/master/docs/_images/calibration_task.png "Depiction of the calibration task.")

*.csv columns*
* **calid** - calibration ID, refere to figure 1
* **pid** - ID of the participant, that performed the calibration
* **indexFinger.X**, **indexFinger.Y**, **indexFinger.Z** - the position of the index finger
* **hand.X**, **hand.Y**, **hand.Z** - the position of the hand
* **forearm.X**, **forearm.Y**, **forearm.Z** - the position of the forearm
* **upperArm.X**, **upperArm.Y**, **upperArm.Z** - the position of the upper arm
* **rightShoulder.X**, **rightShoulder.Y**, **rightShoulder.Z** - the position of the right shoulder
* **hmd.X**, **hmd.Y**, **hmd.Z** - the position of the HMD
* **leftShoulder.X**, **leftShoulder.Y**, **leftShoulder.Z** - the position of the left shoulder
* **indexFingerO.X**, **indexFingerO.Y**, **indexFingerO.Z** - the orientation of the index finger
* **handO.X**, **handO.Y**, **handO.Z** - the orientation of the hand
* **forearmO.X**, **forearmO.Y**, **forearmO.Z** - the orientation of the forearm
* **upperArmO.X**, **upperArmO.Y**, **upperArmO.Z** - the orientation of the upper arm
* **rightShoulderO.X**, **rightShoulderO.Y**, **rightShoulderO.Z** - the orientation of the right shoulder
* **hmdO.X**, **hmdO.Y**, **hmdO.Z** - the orientation of the HMD
* **leftShoulderO.X**, **leftShoulderO.Y**, **leftShoulderO.Z** - the orientation of the left shoulder

### targets.csv

At last, the *targets.csv*-file contains information about which target the participant pointed at for each repetition. The collection ID (**cid**) describes  which collected pointing movement fits to this target and the participant ID (**pid**) the participant that performed the movement. Note again that the combination of these two fields is unique, while the individual fields are not.

*.csv columns*
* **cid** - collection ID
* **pid** - participant ID
* **target.X**, **target.Y**, **target.Z** - the position of the target, see also the target grid in the beginning