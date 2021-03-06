#include "FirstIncludes.h"
#include <stdio.h>
#include <ctype.h>
#include <time.h>
#include <string>
#include <iostream>
#include <fstream>
#include <map>
#include <queue>
#include <stack>
#include <vector>
#ifdef WIN32
#include <windows.h>
#endif
#include "MemoryDebug.h"
using namespace std;


#include "KKBaseTypes.h"
using namespace KKB;


#include "SipperImage.h"
using namespace  ImageExtractionManager;


SipperImage::SipperImage ():
   blobList   (),
   byteOffset (0),
   colLeft    (int32_max),
   colRight   (0),
   imageArea  (0.0f),
   pixelCount (0),
   rowBot     (0),
   rowTop     (int32_max)

{
}


SipperImage::SipperImage (LogicalFrameBlobPtr  blob):
   blobList   (),
   byteOffset (0),
   colLeft    (int32_max),
   colRight   (0),
   imageArea  (0.0f),
   pixelCount (0),
   rowBot     (0),
   rowTop     (int32_max)

{
  LogicalFrameBlob::NeighborsList*  neighbors = NULL;
  LogicalFrameBlob::NeighborsList::iterator  idx;
  LogicalFrameBlobPtr  neighborBlob = NULL;
  stack<LogicalFrameBlobPtr>  toBeProcessed;
  toBeProcessed.push (blob);

  while  (toBeProcessed.size () > 0)
  {
    blob = toBeProcessed.top ();
    toBeProcessed.pop ();
    blobList.push_back (blob);
    colLeft  = Min (colLeft,  blob->colLeft);
    colRight = Max (colRight, blob->colRight);
    rowTop   = Min (rowTop,   blob->rowTop);
    rowBot   = Max (rowBot,   blob->rowBot);
    pixelCount += blob->pixelCount;
    blob->explored = true;

    neighbors = &(blob->Neighbors ());
  
    for  (idx = neighbors->begin ();  idx != neighbors->end ();  idx++)
    {
      neighborBlob = idx->second;
      if  (!neighborBlob->Explored ())
        toBeProcessed.push (neighborBlob);
    }
  }
}




SipperImage::~SipperImage ()
{
}





void  SipperImage::AddBlob (LogicalFrameBlobPtr  _blob)
{
  blobList.push_back (_blob);
  colLeft  = Min (colLeft,  _blob->ColLeft  ());
  colRight = Max (colRight, _blob->ColRight ());
  rowTop   = Min (rowTop,   _blob->RowTop   ());
  rowBot   = Max (rowBot,   _blob->RowBot   ());
  pixelCount += _blob->PixelCount ();
}  /* AddBlob */





void  SipperImage::Explore (LogicalFrameBlobPtr  _blob)
{
  if  (_blob->Explored ())
    return;

  AddBlob (_blob);
  _blob->Explored (true);

  LogicalFrameBlob::NeighborsList&  neighbors = _blob->Neighbors ();
  
  LogicalFrameBlob::NeighborsList::iterator  idx;
  for  (idx = neighbors.begin ();  idx != neighbors.end ();  idx++)
  {
    LogicalFrameBlobPtr  neighborBlob = idx->second;
    if  (!neighborBlob->Explored ())
      Explore (neighborBlob);
  }
}  /* Explore */




void  SipperImage::Dialate (uchar** frame,
                            int**   blobIds,
                            int     size
                           )
{
  vector<LogicalFrameBlobPtr>::iterator  idx;
  for  (idx = blobList.begin ();  idx != blobList.end ();  idx++)
  {
    LogicalFrameBlobPtr  blob = *idx;
    blob->DialateBlob (frame, blobIds, size);
  }
}  /* Dilate */




RasterSipperPtr  SipperImage::GetRaster (uchar**  frame,
                                         int**    blobIds,     
                                         uint     firstFrameRowScanLine,
                                         kkuint64*  frameRowByteOffsets
                                        )

{
  uint   col;
  uint   row;

  uint   numOfRows;
  uint   numOfCols;

  uint  colOffset;
  uint  rowOffset;
  uint  bmpRow;
  uint  bmpCol;

  {
    numOfRows = rowBot   - rowTop  + 17;
    numOfCols = colRight - colLeft + 1;

    rowOffset = 8;

    int padding = 16;

    colOffset = padding / 2;

    numOfCols = numOfCols + padding;
  }

  byteOffset = frameRowByteOffsets[rowTop];

  RasterSipperPtr  r = new RasterSipper (numOfRows, numOfCols, false);  // false = GrayScale image
  //r->BackgroundPixelValue (255);
  //r->ForegroundPixelValue (0);
  uchar** grayArea = r->Green ();

  vector<LogicalFrameBlobPtr>::iterator  idx;
  for  (idx = blobList.begin ();  idx != blobList.end ();  idx++)
  {
    LogicalFrameBlobPtr  blob = *idx;

    uint  blobRowTop   = blob->RowTop   ();
    uint  blobRowBot   = blob->RowBot   ();
    uint  blobColLeft  = blob->ColLeft  ();
    uint  blobColRight = blob->ColRight ();

    uint  blobRowOffset = blobRowTop  - rowTop  + rowOffset;
    uint  blobColOffset = blobColLeft - colLeft + colOffset;

    int  blobId = blob->Id ();

    bmpRow = blobRowOffset;

    for  (row = blobRowTop;  row <= blobRowBot;  row++)
    {
      bmpCol = blobColOffset;

      for  (col = blobColLeft;  col <= blobColRight;  col++)
      {
        if  (blobIds[row][col] == blobId)
          grayArea[bmpRow][bmpCol] = frame[row][col];
        bmpCol++;
      }

      bmpRow++;
    }
  }

  return r;
}  /* GetRaster */


