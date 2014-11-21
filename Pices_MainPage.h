
/**
*@mainpage  Pices
 * 
 *@section  Introduction
 *
 * The Pices system is a set of applications that were developed as part of my Graduate work. I 
 * worked cloely with my major professors Dr. Dmitry Goldgof and Dr. Lawrence Hall and Marine 
 * Scientist Dr. Andrew Remsen.  This work was originally done to process the imagery and 
 * instrumentation data collected by the SIPPER imaging platform developed by University of 
 * South Florida Marine Science Center of Oceans Technology. It manages data from cruises dating 
 * back to 2002 through 2014 and is being updated to work with the latest implementation of SIPPER 
 * for future deployments.<br>
 *
 * The system consists of several applications and libraries.  Most of the code is written in c++
 * and is meant to be O/S neutral. The GUI functionality is written in .net c#.  There is extensive
 * use of Image Processing (IP) and Machine Learning (ML) techniques used in the processing of data.
 * 
 *
 *@section  Applications
 * There are more than 20 applications in the system but only a few comprise the bulk of the
 * functionality
 *
 * List of some but not all applications:
 * - PicesComander  The main GUI application that provides a thumbnail view of collected plankton images; 
 *                  Provides charting functionality, classification processes, reporting facilities allowing 
 *                  user to analyze data by deployments, stations, instrument  parameters(O^2, Depth, Salinity,
 *                  etc...).  Other functionality consists of validation of data, updating of training 
 *                  libraries(Active Learning), and reclassification of imagery based off user improved
 *                  training data.  Randomly extract data from deployments for ground-truthing to
 *                  help gauge classification performance,
 *
 * - WindowsImageExtraction Where raw sipper data is fed into the system; this application extracts imagery, 
 *                  Instrumentation data, computes feature vectors m and performs initial classification that 
 *                  is then fed into the Pices database where the user can then browse the results.
 *
 * - SipperFleViewer - A simple viewer application that allows the user to view the raw sipper data, both
 *                  imagery and instrumentation data.
 *
 * - GradeTraingModel GUI Tool to grade the different training libraries against user ground truthed data;
 *                  generates reports where user can select the classification granularity with respect to
 *                  class hierarchy.
 *
 * - ImportGPS_Data Imports GPS data collected by ship into Pices database.
 *
 * - SpatialAnalysis 
 *
 * - CrossValodation
 *
 * - FeatureSelection - Application that will tune SVM parameters and image extracted features to 
 *                   maximize classification accuracy;  it was decide to run on a cluster;  the more
 *                   CPU's the faster it processes.  It can work in a mixed Windows/linux environment 
 *                   only requiring a common file share device.
 * 
 * - SipperInterface GUI Application that is usd to operate the SIPPER III imaging platform.
 *
 * - SipperDiskManager - GUI Application that manages the downloading of data from the SIPPER III 
 *                      Proprietary disk format.  The forthcoming SIPPER IV will be using standard 
 *                      linux volumes eliminating the need for this application. 
 *
 *@section  Libraries
 * There are several libraries that are utilized by the various applications.
 *
 *List of Libraries:
 * - BaseLibrary(c++)  Functionality that is common to other libraries and applications. Some examples 
 *                includes string management, Matrix operations, Image Processing, Token Parsing, 
 *                Statistics, Histogramming, and common operating system routines. Most O/S specific 
 *                code is implemented in the module "osServices.cpp". This is an earlier version of the 
 *                of the KKBase libray found in "KSquareLibraries".
 *
 * - SipperIstruments(c++) Classes and functions that ar specific to the SIPPER platform are maintained in
 *                this library.  Examples are a group of classes meant to process the different SIPPER
 *                file formats going back to the original SIPPER I platform.
 * 
 * - PicesLibary(c++)  Classes and Code that support Machine-Learning and specific Pices imaging routines.  Other 
 *                functionality includes MySQL DataBase. 
 *     -# FeatureFileIO  several comman feature data file formats are support.  (sparse, arff, c45, etc...)
 *     -# Training-Model-Configuration  class and routines that maintain classifier parameters; such as classifier type.
 *     -# Machine Learning Classes (MLClass)  and containers for tracking lists of classes.
 *     -# Hierarchial Class naming is supported.
 *     -# Containers for Feature Data "FeatureVector", "FeatureVectorList", stratifying by class.
 *     -# Feature data normalization routines.
 *     -# Confusion-Matrix
 *     -# CrossValidation - Example 10 fold CV; also (N x X) cross Validation; typically used by grading a classifier.
 *     -# Implementations of Classifiers;  Pair-Wise Feature selected SVM, Common Features SVM, UsfCasCor, and Dual.
 *
 * - PicesInterface(cli/c++) A mixed Managed/Unmanaged memory model library its purpose is to provide a interfacew
 *                           from the ".net" world to the unmanaged world of the O/S neutral c++ code.
 *
 * - ImageExtractionManager(c++) A frame work for extractingwwwww and classifying SIPPER imagery data; it will process
 *                           the raw SIPPER data using multiple threads to maximize throughput.  
 *
 * - SipperFile(c#) .net classes used by the different GUI applications.
 * 
 * - SipperDiskUtilities (c#) Code that interfaces with the Sipper III hard disk.  Provides routines that make the 
 *                            SipperFles contained on the disk to appear as normal binary data files that can be 
 *                            opened by any .net application.
 * 
 *
 *@section  OutsideLibraries 
 *  I make use of several publicly available libraries:
 *  - <a href="http://www.fftw.org/">FFTW</a> The Fastest Fourier Transform in the West.
 *  - <a href="http://www.zlib.net/">zLib</a>  Used to compress images before storing in database.
 *  - <a href="http://www.csie.ntu.edu.tw/~cjlin/libsvm/">LibSVM</a>  Support Vector Machine.
 */
