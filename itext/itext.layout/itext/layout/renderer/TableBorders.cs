using System.Collections.Generic;
using iText.IO.Log;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class TableBorders {
        protected internal IList<IList<Border>> horizontalBorders;

        protected internal IList<IList<Border>> verticalBorders;

        protected internal Border[] tableBoundingBorders = null;

        protected internal Border[] headerBoundingBorders = null;

        protected internal Border[] footerBoundingBorders = null;

        protected internal IList<CellRenderer[]> rows;

        protected internal int numberOfColumns;

        protected internal int horizontalBordersIndexOffset = 0;

        protected internal int verticalBordersIndexOffset = 0;

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns) {
            //    protected LayoutResult[] splits;
            //
            //    protected boolean hasContent;
            //
            //    protected boolean cellWithBigRowspanAdded;
            //
            //    protected int splitRow;
            this.rows = rows;
            this.numberOfColumns = numberOfColumns;
        }

        // region collapse methods
        protected internal virtual iText.Layout.Renderer.TableBorders CollapseAllBordersAndEmptyRows(IList<CellRenderer
            []> rows, Border[] tableBorders, int startRow, int finishRow, int colN) {
            CellRenderer[] currentRow;
            int[] rowsToDelete = new int[colN];
            for (int row = startRow; row <= finishRow; row++) {
                currentRow = rows[row];
                bool hasCells = false;
                for (int col = 0; col < colN; col++) {
                    if (null != currentRow[col]) {
                        int colspan = (int)currentRow[col].GetPropertyAsInteger(Property.COLSPAN);
                        PrepareBuildingBordersArrays(currentRow[col], tableBorders, colN, row, col);
                        BuildBordersArrays(currentRow[col], row, col);
                        hasCells = true;
                        if (rowsToDelete[col] > 0) {
                            int rowspan = (int)currentRow[col].GetPropertyAsInteger(Property.ROWSPAN) - rowsToDelete[col];
                            if (rowspan < 1) {
                                ILogger logger = LoggerFactory.GetLogger(typeof(TableRenderer));
                                logger.Warn(iText.IO.LogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING);
                                rowspan = 1;
                            }
                            currentRow[col].SetProperty(Property.ROWSPAN, rowspan);
                        }
                        for (int i = 0; i < colspan; i++) {
                            rowsToDelete[col + i] = 0;
                        }
                        col += colspan - 1;
                    }
                    else {
                        if (horizontalBorders[row].Count <= col) {
                            horizontalBorders[row].Add(null);
                        }
                    }
                }
                if (!hasCells) {
                    rows.Remove(currentRow);
                    row--;
                    finishRow--;
                    for (int i = 0; i < colN; i++) {
                        rowsToDelete[i]++;
                    }
                    if (row == finishRow) {
                        ILogger logger = LoggerFactory.GetLogger(typeof(TableRenderer));
                        logger.Warn(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                    }
                }
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders CorrectTopBorder() {
            return CorrectTopBorder(null);
        }

        protected internal virtual iText.Layout.Renderer.TableBorders CorrectTopBorder(iText.Layout.Renderer.TableBorders
             headerTableBorders) {
            IList<Border> topBorders;
            if (null != headerTableBorders) {
                topBorders = headerTableBorders.horizontalBorders[headerTableBorders.horizontalBorders.Count - 1];
            }
            else {
                topBorders = new List<Border>();
                for (int col = 0; col < numberOfColumns; col++) {
                    topBorders.Add(tableBoundingBorders[0]);
                }
            }
            for (int col = 0; col < numberOfColumns; col++) {
                topBorders[col] = GetCollapsedBorder(horizontalBorders[horizontalBordersIndexOffset][col], topBorders[col]
                    );
            }
            UpdateTopBorder(topBorders, new bool[numberOfColumns]);
            if (null != headerTableBorders) {
                headerTableBorders.UpdateBottomBorder(horizontalBorders[horizontalBordersIndexOffset], new bool[numberOfColumns
                    ]);
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders ProcessSplit(int splitRow, bool split, bool 
            hasContent, bool cellWithBigRowspanAdded) {
            return ProcessSplit(splitRow, split, hasContent, cellWithBigRowspanAdded, null);
        }

        protected internal virtual iText.Layout.Renderer.TableBorders ProcessSplit(int splitRow, bool split, bool 
            hasContent, bool cellWithBigRowspanAdded, iText.Layout.Renderer.TableBorders footerTableBorders) {
            CellRenderer[] currentRow = rows[splitRow];
            CellRenderer[] lastRowOnCurrentPage = new CellRenderer[numberOfColumns];
            CellRenderer[] firstRowOnTheNextPage = new CellRenderer[numberOfColumns];
            int curPageIndex = 0;
            int nextPageIndex = 0;
            int row;
            for (int col = 0; col < numberOfColumns; col++) {
                if (hasContent || (cellWithBigRowspanAdded && null == rows[splitRow - 1][col])) {
                    if (null != currentRow[col]) {
                        if (0 >= curPageIndex) {
                            lastRowOnCurrentPage[col] = currentRow[col];
                            curPageIndex = lastRowOnCurrentPage[col].GetPropertyAsInteger(Property.COLSPAN);
                        }
                        if (0 >= nextPageIndex) {
                            firstRowOnTheNextPage[col] = currentRow[col];
                            nextPageIndex = firstRowOnTheNextPage[col].GetPropertyAsInteger(Property.COLSPAN);
                        }
                    }
                }
                else {
                    if (0 >= curPageIndex) {
                        lastRowOnCurrentPage[col] = rows[splitRow - 1][col];
                        curPageIndex = lastRowOnCurrentPage[col].GetPropertyAsInteger(Property.COLSPAN);
                    }
                    if (0 >= nextPageIndex) {
                        row = splitRow;
                        while (row < rows.Count && null == rows[row][col]) {
                            row++;
                        }
                        if (row == rows.Count) {
                            nextPageIndex = 1;
                        }
                        else {
                            firstRowOnTheNextPage[col] = rows[row][col];
                            nextPageIndex = firstRowOnTheNextPage[col].GetPropertyAsInteger(Property.COLSPAN);
                        }
                    }
                }
                curPageIndex--;
                nextPageIndex--;
            }
            //verticalBordersIndexOffset += splitRow;
            // splitRow += horizontalBordersIndexOffset;
            if (hasContent) {
                if (split) {
                    AddNewHorizontalBorder(horizontalBordersIndexOffset + splitRow + 1, true);
                    // the last row on current page
                    AddNewVerticalBorder(verticalBordersIndexOffset + splitRow, true);
                }
                //verticalBordersIndexOffset++;
                splitRow++;
            }
            if (split) {
                AddNewHorizontalBorder(horizontalBordersIndexOffset + splitRow + 1, false);
            }
            // the first row on the next page
            // here splitRow is the last horizontal border index on current page
            // and splitRow + 1 is the first horizontal border index on the next page
            IList<Border> lastBorderOnCurrentPage = horizontalBorders[horizontalBordersIndexOffset + splitRow];
            for (int col = 0; col < numberOfColumns; col++) {
                if (null != lastRowOnCurrentPage[col]) {
                    CellRenderer cell = lastRowOnCurrentPage[col];
                    Border cellModelBottomBorder = GetCellSideBorder(((Cell)cell.GetModelElement()), Property.BORDER_BOTTOM);
                    Border cellCollapsedBottomBorder = GetCollapsedBorder(cellModelBottomBorder, tableBoundingBorders[2]);
                    // fix the last border on the page
                    for (int i = col; i < col + cell.GetPropertyAsInteger(Property.COLSPAN); i++) {
                        lastBorderOnCurrentPage[i] = cellCollapsedBottomBorder;
                    }
                    col += lastRowOnCurrentPage[col].GetPropertyAsInteger(Property.COLSPAN) - 1;
                }
            }
            if (horizontalBordersIndexOffset + splitRow != horizontalBorders.Count - 1) {
                IList<Border> firstBorderOnTheNextPage = horizontalBorders[horizontalBordersIndexOffset + splitRow + 1];
                for (int col = 0; col < numberOfColumns; col++) {
                    if (null != firstRowOnTheNextPage[col]) {
                        CellRenderer cell = firstRowOnTheNextPage[col];
                        Border cellModelTopBorder = GetCellSideBorder(((Cell)cell.GetModelElement()), Property.BORDER_TOP);
                        Border cellCollapsedTopBorder = GetCollapsedBorder(cellModelTopBorder, tableBoundingBorders[0]);
                        // fix the last border on the page
                        for (int i = col; i < col + cell.GetPropertyAsInteger(Property.COLSPAN); i++) {
                            firstBorderOnTheNextPage[i] = cellCollapsedTopBorder;
                        }
                        col += lastRowOnCurrentPage[col].GetPropertyAsInteger(Property.COLSPAN) - 1;
                    }
                }
            }
            // update row offest
            if (split) {
                horizontalBordersIndexOffset += splitRow + 1;
                verticalBordersIndexOffset += splitRow;
            }
            return this;
        }

        // important to invoke on each new page
        private void UpdateFirstRowBorders(int colN) {
            int col = 0;
            int row = 0;
            IList<Border> topBorders = horizontalBorders[0];
            topBorders.Clear();
            while (col < colN) {
                if (null != rows[row][col]) {
                    // we may have deleted collapsed border property trying to process the row as last on the page
                    Border collapsedBottomBorder = null;
                    int colspan = (int)rows[row][col].GetPropertyAsInteger(Property.COLSPAN);
                    for (int i = col; i < col + colspan; i++) {
                        topBorders.Add(rows[row][col].GetBorders()[0]);
                        collapsedBottomBorder = GetCollapsedBorder(collapsedBottomBorder, horizontalBorders[row + 1][i]);
                    }
                    rows[row][col].SetBorders(collapsedBottomBorder, 2);
                    col += colspan;
                    row = 0;
                }
                else {
                    if (0 == row) {
                        horizontalBorders[1][col] = Border.NO_BORDER;
                    }
                    row++;
                    if (row == rows.Count) {
                        break;
                    }
                }
            }
        }

        // collapse with table border or header bottom borders
        protected internal virtual void CorrectFirstRowTopBorders(Border tableBorder, int colN) {
        }

        //        int col = 0;
        //        int row = 0;
        //        List<Border> topBorders = horizontalBorders.get(0);
        //        List<Border> bordersToBeCollapsedWith = null != headerRenderer
        //                ? headerRenderer.horizontalBorders.get(headerRenderer.horizontalBorders.size() - 1)
        //                : new ArrayList<Border>();
        //        if (null == headerRenderer) {
        //            for (col = 0; col < colN; col++) {
        //                bordersToBeCollapsedWith.add(tableBorder);
        //            }
        //        }
        //        col = 0;
        //        while (col < colN) {
        //            if (null != rows.get(row)[col] && row + 1 == (int) rows.get(row)[col].getPropertyAsInteger(Property.ROWSPAN)) {
        //                Border oldTopBorder = rows.get(row)[col].getBorders()[0];
        //                Border resultCellTopBorder = null;
        //                Border collapsedBorder = null;
        //                int colspan = (int) rows.get(row)[col].getPropertyAsInteger(Property.COLSPAN);
        //                for (int i = col; i < col + colspan; i++) {
        //                    collapsedBorder = getCollapsedBorder(oldTopBorder, bordersToBeCollapsedWith.get(i));
        //                    if (null == topBorders.get(i) || (null != collapsedBorder && topBorders.get(i).getWidth() < collapsedBorder.getWidth())) {
        //                        topBorders.set(i, collapsedBorder);
        //                    }
        //                    if (null == resultCellTopBorder || (null != collapsedBorder && resultCellTopBorder.getWidth() < collapsedBorder.getWidth())) {
        //                        resultCellTopBorder = collapsedBorder;
        //                    }
        //                }
        //                rows.get(row)[col].setBorders(resultCellTopBorder, 0);
        //                col += colspan;
        //                row = 0;
        //            } else {
        //                row++;
        //                if (row == rows.size()) {
        //                    break;
        //                }
        //            }
        //        }
        //        if (null != headerRenderer) {
        //            headerRenderer.horizontalBorders.set(headerRenderer.horizontalBorders.size() - 1, topBorders);
        //        }
        // endregion
        // region intialisers
        protected internal virtual void InitializeBorders(IList<Border> lastFlushedRowBottomBorder, bool isFirstOnPage
            ) {
            // initialize borders
            if (null == horizontalBorders) {
                horizontalBorders = new List<IList<Border>>();
                horizontalBorders.Add(new List<Border>(lastFlushedRowBottomBorder));
                verticalBorders = new List<IList<Border>>();
            }
            // The first row on the page shouldn't collapse with the last on the previous one
            if (0 != lastFlushedRowBottomBorder.Count && isFirstOnPage) {
                horizontalBorders[0].Clear();
            }
        }

        //endregion
        // region getters
        protected internal virtual Border GetWidestHorizontalBorder(int row) {
            Border theWidestBorder = null;
            if (row >= 0 && row < horizontalBorders.Count) {
                theWidestBorder = GetWidestBorder(horizontalBorders[row]);
            }
            return theWidestBorder;
        }

        protected internal virtual Border GetWidestVerticalBorder(int col) {
            Border theWidestBorder = null;
            if (col >= 0 && col < verticalBorders.Count) {
                theWidestBorder = GetWidestBorder(verticalBorders[col]);
            }
            return theWidestBorder;
        }

        protected internal virtual float GetMaxTopWidth(Border tableBorder) {
            float width = null == tableBorder ? 0 : tableBorder.GetWidth();
            Border widestBorder = GetWidestHorizontalBorder(horizontalBordersIndexOffset);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        protected internal virtual float GetMaxRightWidth(Border tableBorder) {
            float width = null == tableBorder ? 0 : tableBorder.GetWidth();
            Border widestBorder = GetWidestVerticalBorder(verticalBorders.Count - 1);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        protected internal virtual float GetMaxLeftWidth(Border tableBorder) {
            float width = null == tableBorder ? 0 : tableBorder.GetWidth();
            Border widestBorder = GetWidestVerticalBorder(0);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        // TODO Do not use it =)
        protected internal virtual float GetMaxBottomWidth(Border tableBorder) {
            float width = null == tableBorder ? 0 : tableBorder.GetWidth();
            Border widestBorder = GetWidestHorizontalBorder(horizontalBorders.Count - 1);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        protected internal virtual int GetCurrentHorizontalBordersIndexOffset() {
            return horizontalBordersIndexOffset;
        }

        protected internal virtual int GetCurrentVerticalBordersIndexOffset() {
            return verticalBordersIndexOffset;
        }

        //    protected List<List<Border>> getRendererHorizontalBorders(int startIndex, int numOfRows) {
        //        return horizontalBorders.subList(startIndex, startIndex + numOfRows + 1);
        //    }
        //
        //    protected List<List<Border>> getRendererVerticalBorders(int startIndex, int numOfRows) {
        //        // return horizontalBorders.subList(startIndex, startIndex + numOfRows + 1);
        //    }
        // endregion
        // region setters
        protected internal virtual iText.Layout.Renderer.TableBorders SetRows(IList<CellRenderer[]> rows) {
            this.rows = rows;
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetTableBoundingBorders(Border[] borders) {
            if (null == tableBoundingBorders) {
                tableBoundingBorders = new Border[borders.Length];
            }
            for (int i = 0; i < borders.Length; i++) {
                tableBoundingBorders[i] = borders[i];
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetHeaderBoundingBorders(Border[] borders) {
            if (null == headerBoundingBorders) {
                headerBoundingBorders = new Border[borders.Length];
            }
            for (int i = 0; i < borders.Length; i++) {
                headerBoundingBorders[i] = borders[i];
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetFooterBoundingBorders(Border[] borders) {
            if (null == footerBoundingBorders) {
                footerBoundingBorders = new Border[borders.Length];
            }
            for (int i = 0; i < borders.Length; i++) {
                footerBoundingBorders[i] = borders[i];
            }
            return this;
        }

        //endregion
        //region building border arrays
        protected internal virtual void PrepareBuildingBordersArrays(CellRenderer cell, Border[] tableBorders, int
             colNum, int row, int col) {
            Border[] cellBorders = cell.GetBorders();
            int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            if (0 == col) {
                cell.SetProperty(Property.BORDER_LEFT, GetCollapsedBorder(cellBorders[3], tableBorders[3]));
            }
            if (colNum == col + colspan) {
                cell.SetProperty(Property.BORDER_RIGHT, GetCollapsedBorder(cellBorders[1], tableBorders[1]));
            }
        }

        protected internal virtual void BuildBordersArrays(CellRenderer cell, int row, int col) {
            // We should check if the row number is less than horizontal borders array size. It can happen if the cell with
            // big rowspan doesn't fit current area and is going to be placed partial.
            if (row > horizontalBorders.Count) {
                row--;
            }
            int currCellColspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            int nextCellRow;
            int j;
            // consider the cell on the left side of the current one
            if (col != 0 && null == rows[row][col - 1]) {
                j = col;
                do {
                    j--;
                    nextCellRow = row;
                    while (rows.Count != nextCellRow && null == rows[nextCellRow][j]) {
                        nextCellRow++;
                    }
                }
                while (j > 0 && rows.Count != nextCellRow && (j + rows[nextCellRow][j].GetPropertyAsInteger(Property.COLSPAN
                    ) != col || nextCellRow - rows[nextCellRow][j].GetPropertyAsInteger(Property.ROWSPAN) + 1 != row));
                if (j >= 0 && nextCellRow != rows.Count) {
                    CellRenderer nextCell = rows[nextCellRow][j];
                    BuildBordersArrays(nextCell, nextCellRow, true);
                }
            }
            // consider cells under the current one
            j = 0;
            while (j < currCellColspan) {
                nextCellRow = row + 1;
                while (nextCellRow < rows.Count && null == rows[nextCellRow][col + j]) {
                    nextCellRow++;
                }
                if (nextCellRow == rows.Count) {
                    break;
                }
                CellRenderer nextCell = rows[nextCellRow][col + j];
                // otherwise the border was considered previously
                if (row == nextCellRow - nextCell.GetPropertyAsInteger(Property.ROWSPAN)) {
                    BuildBordersArrays(nextCell, nextCellRow, true);
                }
                j += (int)nextCell.GetPropertyAsInteger(Property.COLSPAN);
            }
            // consider cells on the right side of the current one
            if (col + currCellColspan < rows[row].Length) {
                nextCellRow = row;
                while (nextCellRow < rows.Count && null == rows[nextCellRow][col + currCellColspan]) {
                    nextCellRow++;
                }
                if (nextCellRow != rows.Count) {
                    CellRenderer nextCell = rows[nextCellRow][col + currCellColspan];
                    BuildBordersArrays(nextCell, nextCellRow, true);
                }
            }
            // consider current cell
            BuildBordersArrays(cell, row, false);
        }

        protected internal virtual void BuildBordersArrays(CellRenderer cell, int row, bool isNeighbourCell) {
            int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
            int colN = ((Cell)cell.GetModelElement()).GetCol();
            Border[] cellBorders = cell.GetBorders();
            // cell with big rowspan was splitted
            if (row + 1 - rowspan < 0) {
                rowspan = row + 1;
            }
            // consider top border
            for (int i = 0; i < colspan; i++) {
                if (!CheckAndReplaceBorderInArray(horizontalBorders, row + 1 - rowspan, colN + i, cellBorders[0], false) &&
                     !isNeighbourCell) {
                    cell.SetBorders(horizontalBorders[row + 1 - rowspan][colN + i], 0);
                }
            }
            // consider bottom border
            for (int i = 0; i < colspan; i++) {
                if (!CheckAndReplaceBorderInArray(horizontalBorders, row + 1, colN + i, cellBorders[2], true) && !isNeighbourCell
                    ) {
                    cell.SetBorders(horizontalBorders[row + 1][colN + i], 2);
                }
            }
            // process big rowspan
            if (rowspan > 1) {
                int numOfColumns = numberOfColumns;
                for (int k = row - rowspan + 1; k <= row; k++) {
                    IList<Border> borders = horizontalBorders[k];
                    if (borders.Count < numOfColumns) {
                        for (int j = borders.Count; j < numOfColumns; j++) {
                            borders.Add(null);
                        }
                    }
                }
            }
            // consider left border
            for (int j = row - rowspan + 1; j <= row; j++) {
                if (!CheckAndReplaceBorderInArray(verticalBorders, colN, j, cellBorders[3], false) && !isNeighbourCell) {
                    cell.SetBorders(verticalBorders[colN][j], 3);
                }
            }
            // consider right border
            for (int i = row - rowspan + 1; i <= row; i++) {
                if (!CheckAndReplaceBorderInArray(verticalBorders, colN + colspan, i, cellBorders[1], true) && !isNeighbourCell
                    ) {
                    cell.SetBorders(verticalBorders[colN + colspan][i], 1);
                }
            }
            // process big colspan
            if (colspan > 1) {
                for (int k = colN; k <= colspan + colN; k++) {
                    IList<Border> borders = verticalBorders[k];
                    if (borders.Count < row + rowspan) {
                        for (int l = borders.Count; l < row + rowspan; l++) {
                            borders.Add(null);
                        }
                    }
                }
            }
        }

        //endregion
        //region static methods
        /// <summary>Returns the collapsed border.</summary>
        /// <remarks>
        /// Returns the collapsed border. We process collapse
        /// if the table border width is strictly greater than cell border width.
        /// </remarks>
        /// <param name="cellBorder">cell border</param>
        /// <param name="tableBorder">table border</param>
        /// <returns>the collapsed border</returns>
        public static Border GetCollapsedBorder(Border cellBorder, Border tableBorder) {
            if (null != tableBorder) {
                if (null == cellBorder || cellBorder.GetWidth() < tableBorder.GetWidth()) {
                    return tableBorder;
                }
            }
            if (null != cellBorder) {
                return cellBorder;
            }
            else {
                return Border.NO_BORDER;
            }
        }

        private static Border GetCellSideBorder(Cell cellModel, int borderType) {
            Border cellModelSideBorder = cellModel.GetProperty(borderType);
            if (null == cellModelSideBorder && !cellModel.HasProperty(borderType)) {
                cellModelSideBorder = cellModel.GetProperty(Property.BORDER);
                if (null == cellModelSideBorder && !cellModel.HasProperty(Property.BORDER)) {
                    //                cellModelSideBorder = cellModel.getDefaultProperty(borderType); // TODO
                    //                if (null == cellModelSideBorder && !cellModel.hasDefaultProperty(borderType)) {
                    cellModelSideBorder = cellModel.GetDefaultProperty(Property.BORDER);
                }
            }
            //                }
            return cellModelSideBorder;
        }

        private static Border GetWidestBorder(IList<Border> borderList) {
            Border theWidestBorder = null;
            if (0 != borderList.Count) {
                foreach (Border border in borderList) {
                    if (null != border && (null == theWidestBorder || border.GetWidth() > theWidestBorder.GetWidth())) {
                        theWidestBorder = border;
                    }
                }
            }
            return theWidestBorder;
        }

        // endregion
        // region lowlevel logic
        protected internal virtual bool CheckAndReplaceBorderInArray(IList<IList<Border>> borderArray, int i, int 
            j, Border borderToAdd, bool hasPriority) {
            if (borderArray.Count <= i) {
                for (int count = borderArray.Count; count <= i; count++) {
                    borderArray.Add(new List<Border>());
                }
            }
            IList<Border> borders = borderArray[i];
            if (borders.IsEmpty()) {
                for (int count = 0; count < j; count++) {
                    borders.Add(null);
                }
                borders.Add(borderToAdd);
                return true;
            }
            if (borders.Count == j) {
                borders.Add(borderToAdd);
                return true;
            }
            if (borders.Count < j) {
                for (int count = borders.Count; count <= j; count++) {
                    borders.Add(count, null);
                }
            }
            Border neighbour = borders[j];
            if (neighbour == null) {
                borders[j] = borderToAdd;
                return true;
            }
            else {
                if (neighbour != borderToAdd) {
                    if (borderToAdd != null && neighbour.GetWidth() <= borderToAdd.GetWidth()) {
                        if (!hasPriority && neighbour.GetWidth() == borderToAdd.GetWidth()) {
                            return false;
                        }
                        borders[j] = borderToAdd;
                        return true;
                    }
                }
            }
            return false;
        }

        // TODO
        protected internal virtual iText.Layout.Renderer.TableBorders AddNewHorizontalBorder(int index, bool usePrevious
            ) {
            IList<Border> newBorder;
            if (usePrevious) {
                newBorder = (IList<Border>)((List<Border>)horizontalBorders[index]).Clone();
            }
            else {
                newBorder = new List<Border>();
                for (int i = 0; i < numberOfColumns; i++) {
                    newBorder.Add(Border.NO_BORDER);
                }
            }
            horizontalBorders.Add(index, newBorder);
            return this;
        }

        // TODO
        protected internal virtual iText.Layout.Renderer.TableBorders AddNewVerticalBorder(int index, bool usePrevious
            ) {
            for (int i = 0; i < numberOfColumns + 1; i++) {
                verticalBorders[i].Add(index, usePrevious ? verticalBorders[i][index] : Border.NO_BORDER);
            }
            return this;
        }

        // endregion
        // region footer collapsing methods
        protected internal virtual iText.Layout.Renderer.TableBorders UpdateTopBorder(IList<Border> newBorder, bool
            [] useOldBorders) {
            UpdateBorder(horizontalBorders[horizontalBordersIndexOffset], newBorder, useOldBorders);
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders UpdateBottomBorder(IList<Border> newBorder, 
            bool[] useOldBorders) {
            UpdateBorder(horizontalBorders[horizontalBorders.Count - 1], newBorder, useOldBorders);
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders UpdateBorder(IList<Border> oldBorder, IList<
            Border> newBorders, bool[] isOldBorder) {
            for (int i = 0; i < oldBorder.Count; i++) {
                if (!isOldBorder[i]) {
                    oldBorder[i] = newBorders[i];
                }
            }
            return this;
        }
        // endregion
    }
}
