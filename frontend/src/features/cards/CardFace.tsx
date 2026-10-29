import { useMemo } from "react";
import { useTranslation } from "react-i18next";
import QRCode from "qrcode";
import "./cards.css";

export interface CardFaceProps {
  token: string;
  centreName: string;
  groupName: string;
  studentName: string;
}

/** Generate the matrix locally and render SVG directly. No image URL or network renderer. */
export function CardFace({ token, centreName, groupName, studentName }: CardFaceProps) {
  const { t } = useTranslation("cards");
  const qr = useMemo(() => {
    const { modules } = QRCode.create(token, { errorCorrectionLevel: "M" });
    const paths: string[] = [];
    for (let row = 0; row < modules.size; row++) {
      for (let column = 0; column < modules.size; column++) {
        if (modules.get(row, column)) paths.push(`M${String(column + 4)} ${String(row + 4)}h1v1h-1z`);
      }
    }
    return { size: modules.size + 8, path: paths.join("") };
  }, [token]);
  return (
    <article className="card-face" aria-label={t("faceLabel", { name: studentName })}>
      <div className="card-names">
        <p className="card-centre" dir="auto">{centreName}</p>
        <p className="card-group" dir="auto">{groupName}</p>
        <p className="card-student" dir="auto" style={{ fontSize: studentName.length > 75 ? "2.6mm" : "3.4mm" }}>{studentName}</p>
      </div>
      <div className="card-machine" dir="ltr">
        <svg className="card-qr" role="img" aria-label={t("qrLabel")} viewBox={`0 0 ${String(qr.size)} ${String(qr.size)}`} shapeRendering="crispEdges">
          <rect width={qr.size} height={qr.size} fill="#fff" />
          <path d={qr.path} fill="#000" />
        </svg>
        <p className="card-number">{t("number")} <bdi dir="ltr">{token.slice(5, 11)}</bdi></p>
      </div>
    </article>
  );
}
