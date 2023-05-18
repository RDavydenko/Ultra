import React, { FC, ReactNode, useEffect, useRef, useState } from "react";
import PropTypes from "prop-types";

export interface InfiniteScrollReverseProps {
  className?: string;
  children?: ReactNode[];
  hasMore: boolean;
  isLoading: boolean;
  loadMore: (pageNumber: number) => void;
  loadArea?: number;
  style?: React.CSSProperties;
  setRef?: (element: HTMLDivElement) => void;
}

const InfiniteScrollReverse: FC<InfiniteScrollReverseProps> = ({
  className,
  isLoading,
  hasMore,
  loadArea,
  loadMore,
  children,
  style,
  setRef,
}) => {
  const infiniteRef = useRef<HTMLDivElement>();
  const [currentPage, setCurrentPage] = useState(1);
  const [scrollPosition, setScrollPosition] = useState(0); // Reset default page, if children equals to 0

  const getChildren = () => children ?? [];
  const getClassName = () => className ?? "InfiniteScrollReverse";
  const getLoadArea = () => loadArea ?? 30;

  useEffect(() => {
    infiniteRef.current && setRef?.(infiniteRef.current);
  }, [infiniteRef.current, setRef]);

  useEffect(() => {
    if (getChildren().length === 0) {
      setCurrentPage(1);
    }
  }, [children?.length]);

  useEffect(() => {
    let scrollContainer = infiniteRef.current;

    function onScroll() {
      if (!scrollContainer) return;

      // Handle scroll direction
      if (scrollContainer.scrollTop > scrollPosition) {
        // Scroll bottom
      } else {
        // Check load more scroll area
        if (scrollContainer.scrollTop <= getLoadArea() && !isLoading) {
          // Check for available data
          if (hasMore) {
            // Run data fetching
            const nextPage = currentPage + 1;
            setCurrentPage(nextPage);
            loadMore(nextPage);
          }
        }
      } // Save event scroll position

      setScrollPosition(scrollContainer.scrollTop);
    }

    scrollContainer?.addEventListener("scroll", onScroll);
    return () => {
      scrollContainer?.removeEventListener("scroll", onScroll);
    };
  }, [currentPage, hasMore, isLoading, loadArea, loadMore, scrollPosition]);

  useEffect(() => {
    let scrollContainer = infiniteRef.current;

    if (!scrollContainer) return;

    if (getChildren().length) {
      // Get available top scroll
      const availableScroll =
        scrollContainer.scrollHeight - scrollContainer.clientHeight; // Get motion for first page

      if (currentPage === 1) {
        // Move data to bottom for getting load more area
        if (availableScroll >= 0) {
          scrollContainer.scrollTop = availableScroll;
        }
      } else {
        // Add scroll area for other pages
        if (hasMore) {
          scrollContainer.scrollTop = scrollContainer.clientHeight;
        }
      }
    }
  }, [children?.length, currentPage, hasMore]);

  return /*#__PURE__*/ React.createElement(
    "div",
    {
      className: getClassName(),
      ref: infiniteRef,
      style: style,
    },
    getChildren()
  );
};

export default InfiniteScrollReverse;
