import "@bitnoi.se/react-scheduler/dist/style.css";
import {
  Scheduler,
  SchedulerData,
  SchedulerProjectData,
} from "@bitnoi.se/react-scheduler";

import dayjs from "dayjs";
import isBetween from "dayjs/plugin/isBetween";
import axios from "axios";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";

dayjs.extend(isBetween);
import { SetStateAction, useCallback, useState, useEffect } from "react";

type SchedulerRowLabel = {
  icon: string;
  title: string;
  subtitle: string;
};

export function Calendar() {
  const [isLoading, setIsLoading] = useState(true);
  const [open, setOpen] = useState(false);
  const [isTileData, setIsTileData] = useState(false);
  const [tileData, setTileData] = useState<SchedulerProjectData>();
  const [itemData, setItemData] = useState<SchedulerRowLabel>();

  const [schedulerData, setSchedulerData] = useState<SchedulerData>();

  const [range, setRange] = useState({
    startDate: new Date(),
    endDate: new Date(),
  });

  const handleTileClick = (tile: SchedulerProjectData) => {
    setIsTileData(true);
    setTileData(tile);
    setOpen(true);
  };

  const handleItemClick = (item: SchedulerRowLabel) => {
    setIsTileData(false);
    setItemData(item);
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setTileData(undefined);
    setItemData(undefined);
  };

  useEffect(() => {
    const fetchSchedulerData = async () => {
      setIsLoading(true);
      try {
        let response;
        if (process.env.NODE_ENV === "development") {
          response = await axios.get("http://localhost:5150/api/Calendar");
        } else {
          response = await axios.get("/calendarapi/api/Calendar");
        }
        const data = response.data.map((property: any) => ({
          ...property,
          data: property.data.map((reservation: any) => ({
            ...reservation,
            startDate: new Date(reservation.startDate),
            endDate: new Date(reservation.endDate),
          })),
        }));
        setSchedulerData(data);
      } catch (error) {
        console.error("Error fetching scheduler data:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchSchedulerData();
  }, []);

  const handleRangeChange = useCallback(
    (range: SetStateAction<{ startDate: Date; endDate: Date }>) => {
      setRange(range);
    },
    []
  );

  // Filtering events that are included in current date range
  // Example can be also found on video https://youtu.be/9oy4rTVEfBQ?t=118&si=52BGKSIYz6bTZ7fx
  // and in the react-scheduler repo App.tsx file https://github.com/Bitnoise/react-scheduler/blob/master/src/App.tsx
  const filteredMockedSchedulerData = schedulerData?.map((property) => ({
    ...property,
    data: property.data.filter(
      (project) =>
        // we use "dayjs" for date calculations, but feel free to use library of your choice
        dayjs(project.startDate).isBetween(range.startDate, range.endDate) ||
        dayjs(project.endDate).isBetween(range.startDate, range.endDate) ||
        (dayjs(project.startDate).isBefore(range.startDate, "day") &&
          dayjs(project.endDate).isAfter(range.endDate, "day"))
    ),
  }));

  return (
    <section>
      <Scheduler
        data={filteredMockedSchedulerData || []}
        isLoading={isLoading}
        onRangeChange={handleRangeChange}
        onTileClick={(clickedResource) => handleTileClick(clickedResource)}
        onItemClick={(item) => handleItemClick(item.label)}
        config={{
          zoom: 1,
          filterButtonState: -1,
          includeTakenHoursOnWeekendsInDayView: true,
          showTooltip: false,
          showThemeToggle: true,
        }}
      />
      <Dialog
        open={open}
        onClose={handleClose}
        aria-labelledby="reservation-alert-dialog-title"
        aria-describedby="reservation-alert-dialog-description"
      >
        <DialogTitle id="reservation-alert-dialog-title">
          {"Reservation Info"}
        </DialogTitle>
        <DialogContent>
          {isTileData && (
            <>
              <DialogContentText id="reservation-alert-dialog-description">
                Property name: {tileData?.title}
              </DialogContentText>
              <DialogContentText id="reservation-alert-dialog-description">
                Reservation status: {tileData?.subtitle}
              </DialogContentText>
              {tileData?.description && (
                <DialogContentText id="reservation-alert-dialog-description">
                  Note: {tileData?.description}
                </DialogContentText>
              )}
              <DialogContentText id="reservation-alert-dialog-description">
                Reservation start:{" "}
                {tileData?.startDate
                  ? dayjs(tileData.startDate).format("YYYY-MM-DD")
                  : ""}
              </DialogContentText>
              <DialogContentText id="reservation-alert-dialog-description">
                Reservation end:{" "}
                {tileData?.endDate
                  ? dayjs(tileData.endDate).format("YYYY-MM-DD")
                  : ""}
              </DialogContentText>
            </>
          )}
          {!isTileData && (
            <>
              <DialogContentText id="reservation-alert-dialog-description">
                property name: {itemData?.title}
              </DialogContentText>
              <DialogContentText id="reservation-alert-dialog-description">
                Property owner: {itemData?.subtitle}
              </DialogContentText>
            </>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} autoFocus>
            OK
          </Button>
        </DialogActions>
      </Dialog>
    </section>
  );
}

// const mockedSchedulerData: SchedulerData = [
//   {
//     id: "070ac5b5-8369-4cd2-8ba2-0a209130cc60",
//     label: {
//       icon: "",
//       title: "Property A",
//       subtitle: "Hostaway",
//     },
//     data: [
//       {
//         id: "8b71a8a5-33dd-4fc8-9caa-b4a584ba3762",
//         startDate: new Date("2024-12-20"),
//         endDate: new Date("2024-12-22"),
//         occupancy: 7200,
//         title: "Hostaway",
//         subtitle: "FR",
//         bgColor: "rgb(254,165,177)",
//         description: "hostnote",
//       },
//     ],
//   },
//   {
//     id: "070ac5b5-8369-4cd2-8ba2-0a209130cc6u",
//     label: {
//       icon: "https://picsum.photos/24",
//       title: "Property B",
//       subtitle: "Uplisting - bedbooka",
//     },
//     data: [
//       {
//         id: "8b71a8a5-33dd-4fc8-9caa-b4a584ba3763",
//         startDate: new Date("2024-12-20"),
//         endDate: new Date("2024-12-27"),
//         occupancy: 7200,
//         title: "Project A",
//         subtitle: "Subtitle A",
//         description: "reservationId",
//       },
//     ],
//   },
//   {
//     id: "070ac5b5-8369-4cd2-8ba2-0a209130cc64",
//     label: {
//       icon: "https://picsum.photos/24",
//       title: "Property C",
//       subtitle: "Hostaway",
//     },
//     data: [
//       {
//         id: "8b71a8a5-33dd-4fc8-9caa-b4a584ba3764",
//         startDate: new Date("2024-12-15"),
//         endDate: new Date("2024-12-22"),
//         occupancy: 7200,
//         title: "Project A",
//         subtitle: "Subtitle A",
//       },
//     ],
//   },
//   {
//     id: "070ac5b5-8369-4cd2-8ba2-0a209130cc65",
//     label: {
//       icon: "https://picsum.photos/24",
//       title: "Property D",
//       subtitle: "Uplisting - bedbooka",
//     },
//     data: [
//       {
//         id: "8b71a8a5-33dd-4fc8-9caa-b4a584ba3765",
//         startDate: new Date("2024-12-16"),
//         endDate: new Date("2024-12-30"),
//         occupancy: 7200,
//         title: "Project A",
//         subtitle: "Subtitle A",
//         bgColor: "blue",
//       },
//     ],
//   },
// ];
