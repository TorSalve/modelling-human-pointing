---
# Feel free to add content and custom Front Matter to this file.
# To modify the layout, see https://jekyllrb.com/docs/themes/#overriding-theme-defaults
title: Modelling human pointing
---

# Abstract

<button name="button" onclick="window.open('https://github.com/TorSalve/pointing-in-vr/blob/master/thesis.pdf', '_blank')" style="cursor: pointer">Full thesis (.pdf)</button>

# The collection
To archive the goal of modelling the natural human pointing movement, we build an application that utilizes VR to collect data from participants. The application is build in Unity 2019.2.6f1 and available in this repositiory. Unfortunately we can not provide the complete application, because of copyright reasons. Please add [RootMotion's Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290) to the Assets folder in the Unity project to reconstruct the application.

## Resources
- [HTC Vive Tutorial for Unity](https://www.raywenderlich.com/9189-htc-vive-tutorial-for-unity)

# The data

## Raw collected data
The raw data constists of the collected data. We collect the human pointing movement in 50Hz.
For this experiment we want to collect data, that expresses the movement of a participant, while pointing at a known target. The logs are saved as .csv-files and to avoid saving for instance the participant data repetitively, the data is scattered across different files. All positions are in logged in a y-up fashion.

![Example of a visualization of the collected movement data. Each line represents a sample, while the slightly thicker red line is the final position of the arm. Each point on the line is the position of a rigidbody tracked. This movement took 1.8 seconds to complete.](https://raw.githubusercontent.com/TorSalve/pointing-in-vr/master/docs/_images/plot_projection_3_70_no_target.png "Example of a visualization of the collected movement data. Each line represents a sample, while the slightly thicker red line is the final position of the arm. Each point on the line is the position of a rigidbody tracked. This movement took 1.8 seconds to complete.")
<p style="text-align: center;"><i>Figure 1: Example of a visualization of the collected movement data. Each line represents a sample, while the slightly thicker red line is the final position of the arm. Each point on the line is the position of a rigidbody tracked. This movement took 1.8 seconds to complete.</i></p>

- The participant data is saved to a .csv-file, where each line represents one participant, identified with an unique integer.
- For each repetition we log the movement pattern of the right arm and upper body of the participant, with a frequency of 50Hz. The log for one repetition consists of a number of samples. One sample consists of the position and orientation of each tracked rigidbody and a timestamp, which is the time in seconds after the repetition was started. When the participant finishes the pointing movement, a final sample is logged. Thus each collection of samples fully describe the pointing movement of one task repetition. An example of such a collection is visualized in figure 1. The orientational data is logged as euler angles (a triplet of pitch, yaw and roll). The data is logged such that the positions of rigid bodies are in meters.
- The calibration task is logged by taking a sample, when the participant triggers the calibration. Thus the resulting calibration .csv-file consists of three lines, one for each sample created in the calibration task.
- The target the participant is asked to point at is the denoted the true target. This target is logged in a separate .csv-file for each participant. In this file each line represents the position of a true target and has an identifier, that fits to a collection. 

<button name="button" onclick="window.open('https://github.com/TorSalve/pointing-in-vr/tree/master/data/raw_data', '_blank')" style="cursor: pointer">Check out the raw data</button>


## Normalized data

To make the data comparable, we want to normalize it in some meaningful ways. Without normalization, the data does not compare well, since participants have differently proportioned bodies and might also have slightly different starting positions. These differences are expected, but nevertheless is it still necessary to account for them, since else there might be unrecognized patterns in the data, which may have contributed to a better model if recognized.

### Normalizing by participant height
To account for differences in the participants body proportions, we want to normalize the data by some known measure: the height. We do this by defining a function for each participant:

![Equation 1](https://raw.githubusercontent.com/TorSalve/pointing-in-vr/master/docs/_images/norm_height.png "Equation 1")

where *v* is some positional value in the dataset and *p_v* is the participant *p* that produced the value *v*. This means that these functions transform the positional data to a percentage of the participants height, ie. *f_p(p\[Height\]) = 1*, but note that values may become larger than *1*, eg. when the participant lifts their index finger above their head. This makes data points comparable across participants. There might still be proportional differences, in eg. shoulder height or arm length, but it is difficult to find a normalization that accounts for all the different proportions.

### Moving the starting point
Even though participants where asked to stand on the same spot at the beginning of each repetition, it is unrealistic to expect the same exact starting position in each sample. Thus we want to move the starting point to a more adequate position, which also might not be perfect, but again has better comparability. The basic idea is to move the movement pattern, such that the starting point is centered around (0,0,0) (origin). Intuitively this means that we want to move the participants feet to stand on the origin. But since we do not record the position of the feet, we need to take the next best known position: the head/HMD, since that position is assumed to be centered over the participants feet. We also assume the participant to stand straight at each repetition. When normalizing the data we can use these two assumptions to say that the *x* and *z* values of the feet center-position is equal to the head positions *x* and *z* values. Only the *y* values are different. Thus we can construct a function to rectify the *x* and *z* values:

![Equation 2](https://raw.githubusercontent.com/TorSalve/pointing-in-vr/master/docs/_images/norm_starting.png "Equation 2")

where *e* is an endpoint and *s_e* is the starting point of the pointing movement that resulted in the endpoint *e*.


<button name="button" onclick="window.open('https://github.com/TorSalve/pointing-in-vr/tree/master/data/normalized', '_blank')" style="cursor: pointer">Check out the normalized data</button>

*Note that the data format is different than the raw data, but essentially contains the same data.*


# The analysis
![Figure 3: The target grid, where the colors of the target correspond to their average distance to the predicted target (using SVM), for all folds. The red cross is the participant starting position. Clearly, the target closest to the participant is identified as an outlier. [m]](https://raw.githubusercontent.com/TorSalve/pointing-in-vr/master/docs/_images/plot_distance_targets_SVM.png "Figure 3: The target grid, where the colors of the target correspond to their average distance to the predicted target (using SVM), for all folds. The red cross is the participant starting position. Clearly, the target closest to the participant is identified as an outlier. [m]")
<p style="text-align: center;"><i>Figure 3: The target grid, where the colors of the target correspond to their average distance to the predicted target (using SVM), for all folds. The red cross is the participant starting position. Clearly, the target closest to the participant is identified as an outlier. [m]</i></p>


# Conclusion
This work focuses on natural human pointing movement. Current selection techniques do not allow for both naturalness and selection of distant targets at the same time. Both properties are desirable in VR. We have thus build a model for a selection technique, that both is natural and supports selection of distant targets.

We have collected, analysed and modelled the natural human pointing movement. The resulting model is based on a well performing machine learning model, that predicts target positions based on the collected human pointing movement. The machine learning model encapsulates different factors important to the human pointing, but is limited to the described setup. We theorize that, since we can construct a well performing model, we can extend it to work with a more general setup. The collected data can be used for extending the current model.

With the constructed model we can build a selection technique, that can be used as an alternative to current selection technique and that additionally supports natural human capabilities and knowledge. The model shows the feasibility of modelling human movements and thus opens for new selection techniques, as well as other techniques and applications that are based on human movement.