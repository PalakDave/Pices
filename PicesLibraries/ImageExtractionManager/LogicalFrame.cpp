#include "FirstIncludes.h"
#include <stdio.h>
#include <ctype.h>

#include <time.h>
#include <string>
#include <iostream>
#include <fstream>
#include <map>
#include <vector>
#include "MemoryDebug.h"
using namespace  std;


#include "KKBaseTypes.h"
#include "OSservices.h"
#include "RunLog.h"
#include "KKStr.h"
using namespace  KKB;



#include "LogicalFrame.h"
#include "LogicalFrameBlob.h"
#include "ExtractedImage.h"
#include "SipperImage.h"
using namespace  ImageExtractionManager;



inline  int  LogicalFrame::Max (int x1,  int x2,  int x3, 
                                int x4,  int x5,  int x6
                               )
{
  int  r = KKB::Max (x1, x2);

  if  (x3 > r)  r = x3;
  if  (x4 > r)  r = x4;
  if  (x5 > r)  r = x5;
  if  (x6 > r)  r = x6;

  return  r;
}  /* Max */




inline  int  LogicalFrame::Max (int x1,  int x2,  int x3,  
                                int x4,  int x5,  int x6,  
                                int x7,  int x8,  int x9
                               )
{
  int  r;
  
  if  (x1 > x2)
    r = x1;
  else 
    r = x2;

  if  (x3 > r)  r = x3;
  if  (x4 > r)  r = x4;
  if  (x5 > r)  r = x5;
  if  (x6 > r)  r = x6;
  if  (x7 > r)  r = x7;
  if  (x8 > r)  r = x8;
  if  (x9 > r)  r = x9;

  return  r;
}  /* Max */




/** @brief  Returns the number of different neighbors.  'x1' - 'x9' are the blob id's of the neighbors you want to check. */
int  LogicalFrame::CountPixs (int x1,  int x2,  int x3,  
                              int x4,  int x5,  int x6,  
                              int x7,  int x8,  int x9
                             )
{
  int  count = 0;
  vector<int>  ids;

  if  (x1 >= 0)
    ids.push_back (x1);
  if  (x2 >= 0)
    ids.push_back (x2);
  if  (x3 >= 0)
    ids.push_back (x3);
  if  (x4 >= 0)
    ids.push_back (x4);
  if  (x5 >= 0)
    ids.push_back (x5);
  if  (x6 >= 0)
    ids.push_back (x6);
  if  (x7 >= 0)
    ids.push_back (x7);
  if  (x8 >= 0)
    ids.push_back (x8);
  if  (x9 >= 0)
    ids.push_back (x9);

  if  (ids.size () < 1)
    return 0;

  sort (ids.begin (), ids.end ());
  int lastId = -1;

  for  (KKB::kkuint32 x = 0;  x < ids.size ();  x++)
  {
    if  (ids[x] != lastId)
    {
      count++;
      lastId = ids[x];
    }
  }

  return  count;
}  /* CountPixs */





LogicalFrame::LogicalFrame (const ExtractionParms&  _parms,
                            kkuint32                _frameHeightMax,
                            kkuint32                _frameWidth,
                            const bool&             _cancelFlag
                           ):


  frameHeight                (0),
  frameHeightMax             (_frameHeightMax),
  frameWidth                 (_frameWidth),
  frame                      (NULL),
  frameArea                  (NULL),
  frameSipperRow             (0),
  frameRowByteOffset         (NULL),
  frameTotalPixels           (0),
  pixelsPerRow               (NULL),

  origFrame                  (NULL),
  origFrameArea              (NULL),
  workFrame                  (NULL),
  workFrameArea              (NULL),
  frameNum                   (0),
  imagesInFrame              (0),
  blobs                      (NULL),
  blobIdsArea                (NULL),
  blobIds                    (NULL),
  connectedPixelDist         (_parms.ConnectedPixelDist ()),
  maxBlobsPerFrame           (50000),
  numOfBlobsInFrame          (0),

  parms                      (_parms),

  backgroundPixelTH          (16),

  cropLeft                   (547),
  cropRight                  (1736),
  cancelFlag                 (_cancelFlag)

{
  cropRight = frameWidth - 1;

  if  (parms.FileFormat () == sfSipper4Bit)
  {
    cropLeft = 557;
    cropRight = 1718;
  }

  AllocateFrame ();
}




LogicalFrame::~LogicalFrame ()
{
  DeAllocateFrame ();
}


kkint32  LogicalFrame::MemoryConsumedEstimated ()  const
{
  kkuint32  frameTotalPixels = frameHeightMax * frameWidth;

  kkint32  memoryConsumedEstimated = sizeof (*this) +
         frameTotalPixels                         +  // uchar*          frameArea;
         sizeof (kkuint64*) *  frameHeightMax     +  // kkuint64*       frameRowByteOffset;    
         sizeof (uchar*)    *  frameHeightMax     +  // uchar**         frame;
         sizeof (uchar*)    *  frameHeightMax     +  // uchar**         workFrame;
         frameTotalPixels                         +  // uchar*          workFrameArea;
         sizeof (LogicalFrameBlobPtr) * maxBlobsPerFrame +  //          LogicalFrameBlobPtr*  blobs;
         sizeof (kkint32)   * frameTotalPixels    +  // kkint32*        blobIdsArea;
         sizeof (kkint32*)  * frameHeightMax;        // kkint32**       blobIds;

  if  (origFrame)
  {
    memoryConsumedEstimated = memoryConsumedEstimated +
        sizeof (uchar*)  *  frameHeightMax   +  // uchar**         origFrame;
        frameTotalPixels;                       // uchar*          origFrameArea;
  }

  return  memoryConsumedEstimated;
}  /* MemoryConsumedEstimated */



void  LogicalFrame::AllocateFrame ()
{
  kkuint32 x = 0;

  frameHeightMax = frameWidth;

  frameTotalPixels = frameHeightMax * frameWidth;

  frameArea   = new uchar [frameTotalPixels];
  blobIdsArea = new kkint32 [frameTotalPixels];

  frame              = new uchar* [frameHeightMax];
  frameRowByteOffset = new kkuint64 [frameHeightMax];
  blobIds            = new kkint32* [frameHeightMax];
  pixelsPerRow       = new kkuint32 [frameHeightMax];

  if  (!parms.MorphOperations ().Empty ())
  {
    workFrameArea = new uchar [frameTotalPixels];
    workFrame     = new uchar*[frameHeightMax];
    origFrameArea = new uchar [frameTotalPixels];
    origFrame     = new uchar*[frameHeightMax];
  }

  int  curPixel = 0;

  for  (x = 0; x < frameHeightMax; x++)
  {
    frame[x] = &(frameArea[curPixel]);
    memset (frame[x], 0, frameWidth);
    frameRowByteOffset[x] = 0;

    if  (workFrameArea)
    {
      workFrame[x] = &(workFrameArea[curPixel]);
      memset (workFrame[x], 0, frameWidth);

      origFrame[x] = &(origFrameArea[curPixel]);
      memset (origFrame[x], 0, frameWidth);
    }

    blobIds[x] = &(blobIdsArea[curPixel]);

    pixelsPerRow[x] = 0;
    curPixel += frameWidth;
  }


  blobs = new LogicalFrameBlobPtr[maxBlobsPerFrame];
  for  (x = 0;  x < maxBlobsPerFrame;  x++)
    blobs[x] = new LogicalFrameBlob (frameHeightMax, frameWidth);
}  /* AllocateFrame */




void  LogicalFrame::DeAllocateFrame ()
{
  delete  frameArea;       frameArea     = NULL;
  delete  blobIdsArea;     blobIdsArea   = NULL;

  delete  workFrameArea;   workFrameArea = NULL;
  delete  workFrame;       workFrame     = NULL;

  delete  origFrame;       origFrame     = NULL;
  delete  origFrameArea;   origFrameArea = NULL;

  if  (blobs)
  {
    for  (kkuint32 x = 0;  x < maxBlobsPerFrame;  x++)
    {
      delete  blobs[x];
      blobs[x] = NULL;
    }
    delete  blobs;
    blobs = NULL;
  }

  delete  frame;                frame              = NULL;
  delete  frameRowByteOffset;   frameRowByteOffset = NULL;
  delete  blobIds;              blobIds            = NULL;

  delete  pixelsPerRow;         pixelsPerRow       = NULL;
}



bool  LogicalFrame::ForegroundPixel (uchar  pixValue)
{
  return  (pixValue > backgroundPixelTH);
}  /* ForegroundPixel */




void  LogicalFrame::CountAndTotal (KKB::uchar    c,
                                   KKB::ushort&  count,
                                   KKB::ushort&  total
                                  )
{
  if  (c > backgroundPixelTH)
  {
    count++;
    total += c;
  }
}  /* CountAndTotal */



void  LogicalFrame::PerformDialation ()
{
  kkuint32  row, col;

  uchar*  curRow = NULL;
  uchar*  nextRow = NULL;
  uchar*  lastRow = NULL;
  uchar*  workRow = NULL;

  ushort  count, total;

  for  (row = 1;  row < frameHeight;  row++)
  {
    lastRow = frame[row - 1];
    curRow  = frame[row];
    nextRow = frame[row + 1];
    workRow = workFrame[row];

    for  (col = 1;  col < (frameWidth - 1);  col++)
    {
      if  (curRow[col] > backgroundPixelTH)
      {
        workRow[col] = curRow[col];
      }
      else
      {
        if  ((lastRow[col-1] <= backgroundPixelTH)  &&
             (lastRow[col  ] <= backgroundPixelTH)  &&
             (lastRow[col+1] <= backgroundPixelTH)  &&

             (curRow [col-1] <= backgroundPixelTH)  &&
             (curRow [col+1] <= backgroundPixelTH)  &&

             (nextRow[col-1] <= backgroundPixelTH)  &&
             (nextRow[col  ] <= backgroundPixelTH)  &&
             (nextRow[col+1] <= backgroundPixelTH)
            )
        {
          workRow[col] = 0;
        }
        else
        {
          count = total = 0;
          CountAndTotal (lastRow[col-1], count, total);
          CountAndTotal (lastRow[col  ], count, total);
          CountAndTotal (lastRow[col+1], count, total);

          CountAndTotal (curRow [col-1], count, total);
          CountAndTotal (curRow [col+1], count, total);

          CountAndTotal (nextRow[col-1], count, total);
          CountAndTotal (nextRow[col  ], count, total);
          CountAndTotal (nextRow[col+1], count, total);

          workRow[col] = total / count;
        }
      }
    }
  }

  // Lets copy back
  for  (row = 1;  row < frameHeight;  row++)
  {
    curRow  = frame     [row];
    workRow = workFrame [row];

    for  (col = 1;  col < (frameWidth - 1);  col++)
    {
      curRow[col] = workRow[col];
    }
  }
}  /* PerformDialation */



void  LogicalFrame::PerformErosion ()
{
  kkuint32  row, col;

  uchar*  curRow = NULL;
  uchar*  nextRow = NULL;
  uchar*  lastRow = NULL;
  uchar*  workRow = NULL;

  for  (row = 1; row < frameHeight; row++)
  {
    curRow  = frame[row];
    lastRow = frame[row - 1];
    nextRow = frame[row + 1];
    workRow = workFrame[row];

    for  (col = 1;  col < (frameWidth - 1);  col++)
    {
      if  (curRow[col] <= backgroundPixelTH)
      {
        workRow[col] = 0;
      }
      else
      {
        if  ((lastRow[col-1] <= backgroundPixelTH)  ||
             (lastRow[col  ] <= backgroundPixelTH)  ||
             (lastRow[col+1] <= backgroundPixelTH)  ||

             (curRow [col-1] <= backgroundPixelTH)  ||
             (curRow [col+1] <= backgroundPixelTH)  ||

             (nextRow[col-1] <= backgroundPixelTH)  ||
             (nextRow[col  ] <= backgroundPixelTH)  ||
             (nextRow[col+1] <= backgroundPixelTH)
            )

          workRow[col] = 0;
        else
          workRow[col] = curRow[col];
      }
    }
  }

  // Lets copy back
  for  (row = 1;  row < frameHeight;  row++)
  {
    curRow  = frame     [row];
    workRow = workFrame [row];

    for  (col = 1;  col < (frameWidth - 1);  col++)
    {
      curRow[col] = workRow[col];
    }
  }
}  /* PerformErosion */



void  LogicalFrame::PerformOpening ()
{
  PerformErosion ();
  PerformDialation ();
}  /* PerformOpening */




void  LogicalFrame::PerformClosing ()
{
  PerformDialation ();
  PerformErosion ();
}  /* PerformOpening */




void  LogicalFrame::PerformMorphOperations ()
{
  KKStr  ops = parms.MorphOperations ();

  memcpy (origFrameArea, frameArea, frameTotalPixels);
  
  while  (!ops.Empty ())
  {
    KKStr  nextOp = ops.ExtractToken (",\n\r\t");
    nextOp.Upper ();
    if  (nextOp == "O")
      PerformOpening ();

    else if  (nextOp == "C")
      PerformClosing ();

    else if  (nextOp == "D")
      PerformDialation ();

    else if  (nextOp == "E")
      PerformErosion ();
  }

}  /* PerformMorphOperations */





LogicalFrameBlobPtr  LogicalFrame::NewBlob (kkuint32  rowTop,
                                            kkuint32  colLeft
                                           )
{
  if  (numOfBlobsInFrame >= maxBlobsPerFrame)
  {
    // This is some really crap code that I am adding here.  
    // the whole way that 'blobs' is being managed needs to be 
    // reworked.
    kkuint32  x = 0;

    // Lets go and re-use an older smaller blob

    while  (x < numOfBlobsInFrame)
    {
      if  (blobs[x]->Id () == -1)
      {
        // We found a unused blob that is not being used any more
        
        LogicalFrameBlob&  blob = *(blobs[x]);

        blob.id = x;
        //  numOfBlobsInFrame++;  // We don't increment this since we are making use 
                                  // of an older blob that has been merged.

        blob.rowTop     = rowTop;
        blob.rowBot     = rowTop;

        blob.colLeft    = colLeft;
        blob.colRight   = colLeft; 
        blob.pixelCount = 0;
        blob.explored   = false;
        blob.neighbors.clear ();
        blob.neighborIdLastAdded = -1;
        return  &blob;
      }

      x++;
    }

    {
      // There are no empty slots in 'blobs'   we are going to have to expand it.
      kkuint32  newMaxBlobsPerFrame  = maxBlobsPerFrame + 10000;

      cout << "LogicalFrame::NewBlob    maxBlobsPerFrame[" << maxBlobsPerFrame << "] Exceeded  New Size[" << newMaxBlobsPerFrame << "]." << endl;
      
      LogicalFrameBlobPtr*  newBlobs = new LogicalFrameBlobPtr[newMaxBlobsPerFrame];

      kkuint32  blobIDX = 0;
      while  (blobIDX < maxBlobsPerFrame)
      {
        newBlobs[blobIDX] = blobs[blobIDX];
        blobs[blobIDX] = NULL;
        blobIDX++;
      }

      delete[]  blobs;  blobs = NULL;

      while  (blobIDX < newMaxBlobsPerFrame)
      {
        newBlobs[blobIDX] = new LogicalFrameBlob (frameHeightMax, frameWidth);
        blobIDX++;
      }

      blobs = newBlobs;
      maxBlobsPerFrame = newMaxBlobsPerFrame;
    }
  }

  LogicalFrameBlob&  blob = *(blobs[numOfBlobsInFrame]);

  blob.id = numOfBlobsInFrame;
  numOfBlobsInFrame++;

  blob.rowTop     = rowTop;
  blob.rowBot     = rowTop;

  blob.colLeft    = colLeft;
  blob.colRight   = colLeft; 
  blob.pixelCount = 0;
  blob.explored   = false;
  blob.neighbors.clear ();
  blob.neighborIdLastAdded = -1;

  return  &blob;
}  /* NewBlob */



void  LogicalFrame::InitailzieBlobIds ()
{
  kkuint32  totalPixels = frameHeightMax * frameWidth;
  for  (kkuint32 x = 0;  x < totalPixels;  ++x)
    blobIdsArea[x] = -1;
}  /* InitailzieBlobIds */



void  LogicalFrame::LocateBlobsUsingConnectedDistanceOf3 ()
{
  uchar*         curRow           = NULL;
  kkint32*         curRowBlobIds    = NULL;
  kkint32*         prev1RowBlobIds  = NULL;
  kkint32*         prev2RowBlobIds  = NULL;
  kkint32*         prev3RowBlobIds  = NULL;

  kkuint32       col = 3;
  kkuint32       row = 3;

  LogicalFrameBlobPtr  curBlob    = NULL;
  kkint32        curBlobId  = 0;
  kkint32        nearBlobId = 0;

  kkuint32       blankColsInARow = 0;

  for  (row = connectedPixelDist;  row < frameHeight;  row++)
  {
    col = connectedPixelDist;

    curRow           = frame[row];
    curRowBlobIds    = blobIds[row];
    prev1RowBlobIds  = blobIds[row - 1];
    prev2RowBlobIds  = blobIds[row - 2];
    prev3RowBlobIds  = blobIds[row - 3];

    curBlob = NULL;

    if  (pixelsPerRow[row] > 0)
    {
      while  (col < (frameWidth - 3))
      {
        if  (curRow[col] > backgroundPixelTH)
        {
          blankColsInARow = 0;

          // Check Upper Left 1st
          nearBlobId = Max (prev1RowBlobIds[col],
                            prev2RowBlobIds[col],
                            prev3RowBlobIds[col],
                            prev1RowBlobIds[col - 1],
                            prev2RowBlobIds[col - 1],
                            prev3RowBlobIds[col - 1],
                            prev1RowBlobIds[col - 2],
                            prev2RowBlobIds[col - 2],
                            prev1RowBlobIds[col - 3]
                           );

          if  (nearBlobId < 0)
          {
            // Check Upper Right Pixels.
            nearBlobId = Max (prev1RowBlobIds[col + 3],
                              prev1RowBlobIds[col + 2],
                              prev2RowBlobIds[col + 2],
                              prev1RowBlobIds[col + 1],
                              prev2RowBlobIds[col + 1],
                              prev3RowBlobIds[col + 1]
                             );
          }

          if  (curBlob)
          {
            if  (nearBlobId >= 0)
            {
              // There is an image with-in a couple of pixels

              if  (nearBlobId != curBlobId)
              {
                blobs[nearBlobId]->AddNeighbor (curBlob);
                curBlob->AddNeighbor (blobs[nearBlobId]);
              }
            }

            curRowBlobIds[col] = curBlobId;
            curBlob->colRight  = KKB::Max (curBlob->colRight, col);
            curBlob->rowBot    = KKB::Max (curBlob->rowBot,   row);
            curBlob->colLeft   = KKB::Min (curBlob->colLeft,  col);
            curBlob->rowTop    = KKB::Min (curBlob->rowTop,   row);

            curBlob->pixelCount++;
          }
          else   
          {
            // No Current LogicalFrameBlob
            if  (nearBlobId >= 0)
            {
              curBlob   = blobs[nearBlobId];
              curBlobId = curBlob->id;
            }
            else
            {
              curBlob = NewBlob (row, col);
              curBlobId = curBlob->id;
            }

            curRowBlobIds[col] = curBlobId;
            curBlob->colRight  = KKB::Max (curBlob->colRight, col);
            curBlob->rowBot    = KKB::Max (curBlob->rowBot,   row);

            curBlob->colLeft   = KKB::Min (curBlob->colLeft, col);
            curBlob->rowTop    = KKB::Min (curBlob->rowTop,  row);

            curBlob->pixelCount++;
          }
        }
        else
        {
          if  (curBlob)
          {
            // Check Upper Left 1st
            nearBlobId = Max (prev1RowBlobIds[col],
                              prev2RowBlobIds[col],
                              prev3RowBlobIds[col],
                              prev1RowBlobIds[col - 1],
                              prev2RowBlobIds[col - 1],
                              prev3RowBlobIds[col - 1],
                              prev1RowBlobIds[col - 2],
                              prev2RowBlobIds[col - 2],
                              prev1RowBlobIds[col - 3]
                             );

            if  (nearBlobId >= 0)
            {
              if  (nearBlobId != curBlobId)
              {
                blobs[nearBlobId]->AddNeighbor (curBlob);
                curBlob->AddNeighbor (blobs[nearBlobId]);
              }
            }
          }

          blankColsInARow++;
          if  (blankColsInARow > connectedPixelDist)
          {
            curBlob = NULL;
            curBlobId = -1;
          }
        }

        col++;
      }
    }
  }
}  /* LocateBlobsUsingConnectedDistanceOf3 */




int  LogicalFrame::GetMaxBlobIdInUpperLeft (kkuint32 row,
                                            kkuint32 col
                                           )
{
  int  maxBlobId = -1;
  kkuint32  c = 0;
  kkuint32  r = 0;
  kkuint32  startCol = 0;

  startCol = col - 1;

  for (r = row - connectedPixelDist;  r < row;  r++)
  {
    if  (r >= 0)
    {
      for  (c = startCol;  c <= col;  c++)
      {
        if  (c >= 0)
        {
          if  (blobIds[r][c] > maxBlobId)
            maxBlobId = blobIds[r][c];
        }
      }
    }

    startCol--;
  }

  return  maxBlobId;
}  /* GetMaxBlobIdInUpperLeft */



int  LogicalFrame::GetMaxBlobIdInUpperRight (kkuint32 row,
                                             kkuint32 col
                                            )
{
  kkint32  maxBlobId = -1;
  kkuint32  r = 0;
  kkuint32  c = 0;
  kkuint32  endCol = 0;

  endCol = col + 1;
  for  (r = row - connectedPixelDist;  r < row;  r++)
  {
    if  (r >= 0)
    {
      if  (endCol >= frameWidth)
        endCol = frameWidth - 1;

      for  (c = col + 1;  c <= endCol;  c++)
      {
        if  (blobIds[r][c] > maxBlobId)
          maxBlobId = blobIds[r][c];
      }
    }

    endCol++;
  }

  return  maxBlobId;
}  /* GetMaxBlobIdInUpperRight */




void  LogicalFrame::LocateBlobsUsingConnectedDistanceNotOf3 ()
{
  uchar*         curRow           = NULL;
  kkint32*         curRowBlobIds    = NULL;

  kkuint32       col              = 1;
  kkuint32       row              = 1;

  LogicalFrameBlobPtr  curBlob          = NULL;
  kkint32        curBlobId        = 0;
  kkint32        nearBlobId       = 0;

  kkuint32       blankColsInARow = 0;

  for  (row = 1;  row < frameHeight;  row++)
  {
    col = connectedPixelDist;

    curRow        = frame[row];
    curRowBlobIds = blobIds[row];

    curBlob = NULL;

    if  (pixelsPerRow[row] > 0)
    {
      while  (col < (frameWidth - 3))
      {
        if  (curRow[col] > backgroundPixelTH)
        {
          blankColsInARow = 0;

          // Check Upper Left 1st
          nearBlobId = GetMaxBlobIdInUpperLeft (row, col);

          if  (nearBlobId < 0)
          {
            // Check Upper Right Pixels.
            nearBlobId = GetMaxBlobIdInUpperRight (row, col);
          }

          if  (curBlob)
          {
            if  (nearBlobId >= 0)
            {
              // There is an image with-in a couple of pixels

              if  (nearBlobId != curBlobId)
              {
                // The near LogicalFrameBlob is different than the curBlob;  so the two
                // blobs are really one.  

                blobs[nearBlobId]->AddNeighbor (curBlob);
                curBlob->AddNeighbor (blobs[nearBlobId]);
              }
            }

            curRowBlobIds[col] = curBlobId;
            curBlob->colRight  = KKB::Max (curBlob->colRight, col);
            curBlob->rowBot    = KKB::Max (curBlob->rowBot,   row);
            curBlob->colLeft   = KKB::Min (curBlob->colLeft,  col);
            curBlob->rowTop    = KKB::Min (curBlob->rowTop,   row);

            curBlob->pixelCount++;
          }
          else   
          {
            // No Current LogicalFrameBlob
            if  (nearBlobId >= 0)
            {
              curBlob   = blobs[nearBlobId];
              curBlobId = curBlob->id;
            }
            else
            {
              curBlob = NewBlob (row, col);
              curBlobId = curBlob->id;
            }

            curRowBlobIds[col] = curBlobId;
            curBlob->colRight  = KKB::Max (curBlob->colRight, col);
            curBlob->rowBot    = KKB::Max (curBlob->rowBot,   row);

            curBlob->colLeft   = KKB::Min (curBlob->colLeft, col);
            curBlob->rowTop    = KKB::Min (curBlob->rowTop,  row);

            curBlob->pixelCount++;
          }
        }
        else
        {
          // Background Pixel

          if  (curBlob)
          {
            // Check Upper Left 1st
            nearBlobId = GetMaxBlobIdInUpperLeft (row, col);

            if  (nearBlobId >= 0)
            {
              if  (nearBlobId != curBlobId)
              {
                blobs[nearBlobId]->AddNeighbor (curBlob);
                curBlob->AddNeighbor (blobs[nearBlobId]);
              }
            }
          }

          blankColsInARow++;
          if  (blankColsInARow > connectedPixelDist)
          {
            curBlob = NULL;
            curBlobId = -1;
          }
        }

        col++;
      }
    }
  }  /* row */
}  /* LocateBlobsUsingConnectedDistanceNotOf3 */




VectorSipperImagePtr  LogicalFrame::BuildListOfSipperImages (kkuint32&  imagesInFrame)
{
  VectorSipperImagePtr  sipperImages = new VectorSipperImage ();

  for  (kkuint32 blobIDX = 0;  (blobIDX < numOfBlobsInFrame)  &&  (!cancelFlag);  ++blobIDX)
  {
    LogicalFrameBlobPtr  blob = blobs[blobIDX];
    if  (!blob)
    {
      cerr << endl << endl 
           << "LogicalFrame::BuildListOfSipperImages    ***ERROR***     blob == NULL" << endl
           << endl;
      continue;
    }

    if  (blob->Explored ())
      continue;

    SipperImagePtr  sipperImage = new SipperImage ();
    sipperImage->Explore (blob);

    bool  skipThisImage = false;

    //if  (sipperImage->PixelCount () >= 250)
    //{
    // // We should only do this one time
    //  skipThisImage = true;
    //}
    //else 
    if  (sipperImage->PixelCount () < 2)
    {
      // We will not consider a single pixel an image.
      skipThisImage = true;
    }
    else
    {
      imagesInFrame++;

      kkuint32  pixelsInImage = sipperImage->PixelCount ();
      //     set @sqlStr = concat(@sqlStr, 'fd.FilledArea * (', _chamberWidth, ' / (id.CropRight - id.CropLeft)) * 1000  * (id.FlowRate1 / sf.ScanRate) * 1000.0  as Statistic \n');


      double imageArea = pixelsInImage * areaPerPixel;
      sipperImage->ImageArea ((float)imageArea);


      // This is were we decide if a particular Sipper Image has met the user criteria 
      if  (imageArea < parms.MinAreaSize () || (parms.MaxAreaSize () > parms.MinAreaSize() && (imageArea > parms.MaxAreaSize())))
      {
        skipThisImage = true;
      }
    }

    if  ((!skipThisImage)  &&  (parms.PreProcess ()))
    {
      kkuint32  h = sipperImage->Height ();
      kkuint32  w = sipperImage->Width ();

      // Will Check to see if this is an obvious Vertical Artifact Line.

      if  ((w < 14)  &&  (h > 20))
      {
        float  widthVsHeight = (float)w / (float)h;
        if  (widthVsHeight < 0.08f)
        {
          int  availArea =  w * h;
          float  pixelUsedRatio = (float)sipperImage->PixelCount () / (float)availArea;

          if  (pixelUsedRatio > 0.2f)
          {
            skipThisImage = true;
          }
        }
      }
    }

    if  (skipThisImage)
    {
      delete  sipperImage;
      sipperImage = NULL;
      continue;
    }

    sipperImages->push_back (sipperImage);
  }
  return  sipperImages;
}  /* BuildListOfSipperImages */




ExtractedImageListPtr  LogicalFrame::ProcessFrame ()
{
  if  (cancelFlag)
    return  NULL;

  imagesInFrame = 0;

  numOfBlobsInFrame = 0;

  // Initialize LogicalFrameBlob ID's
  InitailzieBlobIds ();

  int  imageId = -1;

  if  (parms.FileFormat () == sfSipper4Bit)
  {
    // because of noise on left and right we are going to blank out the
    // offending columns.

    for  (kkuint32  row = 0;  row < frameHeight;  row++)
    {
      kkuint32  col = 0;
      uchar*  curRow = frame[row];
      for  (col = 0;  col < cropLeft;  ++col)
        curRow[col] = 0;

      for  (col = cropRight + 1;  col < frameWidth;  ++col)
        curRow[col] = 0;
    }
  }


  ExtractedImageListPtr  extractdImages = new ExtractedImageList (true);

  //if  (connectedPixelDist == 3)
  //  LocateBlobsUsingConnectedDistanceOf3 ();
  //else
  LocateBlobsUsingConnectedDistanceNotOf3 ();

  VectorSipperImagePtr  sipperImages = BuildListOfSipperImages (imagesInFrame);

  if  (origFrameArea)
    memcpy (frameArea, origFrameArea, frameTotalPixels);

  // Now that we found all the blobs in the frame,  lets select 
  // the candidate ones that are to be written out.

  VectorSipperImage::iterator  siIDX;
  for  (siIDX = sipperImages->begin ();   ((siIDX != sipperImages->end ())  &&  (!cancelFlag));  siIDX++)
  {
    SipperImagePtr  sipperImage = *siIDX;

    bool   noiseLine = false;

    if  ((sipperImage->ColLeft () < 350)  ||  (sipperImage->ColLeft () > 3700))
    {
      kkuint32  width  = 1 + sipperImage->ColRight () - sipperImage->ColLeft ();
      kkuint32  height = 1 + sipperImage->RowBot   () - sipperImage->RowTop  ();

      if  ((height > 90)  &&  (width < 30))
        noiseLine = true;

      else if  ((height > 300)  &&   (width < 40))
        noiseLine = true;

      else if  ((height > 600)  &&   (width < 51))
        noiseLine = true;

      else if  ((height > 1500)  &&   (width < 58))
        noiseLine = true;
    }

    if  (noiseLine)
    {
      continue;
    }

    if  (origFrameArea)
      sipperImage->Dialate (frame, blobIds, 2);

    imagesInFrame++;

    // extractedImage will take ownership of 'raster'.
    // extractdImages will take ownership of 'extractedImage'.
    RasterSipperPtr raster = sipperImage->GetRaster (frame, blobIds, frameSipperRow, frameRowByteOffset);
    kkuint32  sipperTopRow = sipperImage->RowTop () + frameSipperRow;
    kkuint32  sipperTopCol = sipperImage->ColLeft ();
    ExtractedImagePtr  extractedImage = new ExtractedImage (raster, 
                                                            sipperImage->ByteOffset (), 
                                                            sipperTopRow, 
                                                            sipperTopCol,
                                                            sipperImage->PixelCount (),
                                                            sipperImage->ImageArea (),
                                                            0.0f,  // Depth
                                                            NULL,  // predClass1
                                                            0.0f,  // predClass2Prob
                                                            NULL,  // predClass2
                                                            0.0f   // predClass2Prob
                                                           );
    extractdImages->PushOnBack (extractedImage);
  }  /* end of for blobIDX */


  {
    // Need to clean up sipperImages  list
    VectorSipperImage::iterator  siIDX;
    for  (siIDX = sipperImages->begin ();  siIDX != sipperImages->end ();  siIDX++)
    {
      SipperImagePtr  sipperImage = *siIDX;
      delete  sipperImage;
    }

    delete  sipperImages;  sipperImages = NULL;
  }

  return  extractdImages;
} /* ProcessFrame */





void  LogicalFrame::PopulateFrame (kkuint32   _frameNum,
                                   kkuint32   _frameHeight,
                                   uchar*     _frameArea,
                                   kkuint32   _frameSipperRow,
                                   kkuint64*  _frameRowByteOffset,
                                   kkuint32*  _pixelsPerRow,
                                   double     _areaPerPixel
                                  )
{
  frameNum       = _frameNum;
  frameHeight    = _frameHeight;
  frameSipperRow = _frameSipperRow;
  areaPerPixel   = _areaPerPixel;

  if  (frameHeight > frameHeightMax)
  {
    // We need to increase the size of this frame.
    DeAllocateFrame ();
    frameHeightMax = frameHeight + 3;
    AllocateFrame ();
  }

  kkuint32  totalPixels = frameHeight * frameWidth;
  memcpy (frameArea, _frameArea, totalPixels);

  for  (kkuint32 row = 0;  row < frameHeight;  ++row)
  {
    frameRowByteOffset[row] = _frameRowByteOffset[row];
    pixelsPerRow      [row] = _pixelsPerRow[row];
  }
}  /* PopulateFrame */

