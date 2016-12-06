<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <html lang="en" xmlns="http://www.w3.org/1999/xhtml">
      <head>
      </head>
      <body>
        <div style="margin-top:0%;width:100%;">

        </div>
        <div style="padding-right:15px;padding-left:15px;margin-right:auto;margin-left:auto;margin-top:3%;border-top:3px solid #ee0000;">
          <div style="background:#fff;height:50px;float: left;">
          </div>
        </div>

        <div style="margin-right: -15px;margin-left: -15px;">
          <div style="width:100%;">
            <p style="font-family: arial, sans-serif;width: 100%;">
              You subscribed to be notified when a fault matching your subscription occurs. The summary for the faults are below :
            </p>
            <table style="font-family: arial, sans-serif;border-collapse: collapse;width: 100%;ffont-size:11px;">
              <caption>
                Application : <xsl:value-of select="ArrayOfAlertFaultSummary/AlertFaultSummary[1]/Application"/>
              </caption>
              <tr>
                <th style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">Service Name</th>
                <th style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">Type</th>
                <th style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">Count</th>
                <th style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">Earliest</th>
                <th style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">Latest</th>
                <th style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;"></th>
              </tr>
              <xsl:for-each select="ArrayOfAlertFaultSummary/AlertFaultSummary">
                <tr>
                  <td style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">
                    <span style="display: block;width: 200px;word-wrap: break-word;">
                      <xsl:value-of select="ServiceName"/>
                    </span>
                  </td>
                  <td style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">
                    <xsl:value-of select="ExceptionType"/>
                  </td>
                  <td style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">
                      <xsl:value-of select="Count"/>
                  </td>
                  <td style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">
                    <xsl:value-of select="MinTime"/>
                  </td>
                  <td style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">
                      <xsl:value-of select="MaxTime"/>
                  </td>
                  <td style="border: 1px solid #dddddd;text-align: left;padding: 8px;font-size: 12px;">
                    <a target="_blank" >
                      <xsl:attribute name="href">
                       {PORTAL_ROOT_PATH}/Esb/Fault?Init=True&amp;Status=UnResolved&amp;Application=<xsl:value-of select="Application"/>&amp;FromDateTime=<xsl:value-of select="MinTime"/>&amp;ToDateTime=<xsl:value-of select="MaxTime"/>
                        </xsl:attribute>
                      Detail
                    </a>
                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </div>
        </div>

        <footer>
          <div style="padding-top:10px;text-align:center;padding-bottom:0;margin-bottom:0">
            <em style="font-size:smaller;">
              This is a notification-only email address. Please do not reply to this message.
            </em>
          </div>

          <div style="padding-right:15px;padding-left:15px;margin-right:auto;margin-left:auto;margin-top:1%;border-top:3px solid #ee0000;">

            <div style="margin-right:-15px;margin-left:-15px;">

              <div style="width:100%;text-align:center">


                <ul style="padding-left:0;margin-left:-5px;list-style:none;margin:10px 0 0 0;">
                  <li style="display: inline-block;padding-right: 5px;padding-left: 5px;">
                    <a style="display: inline-block;margin-bottom: 0;text-align: center;white-space: nowrap;vertical-align: middle;
                            -ms-touch-action: manipulation;touch-action: manipulation;cursor: pointer;-webkit-user-select: none;
                            -moz-user-select: none;-ms-user-select: none;user-select: none;background-image: none;border: 1px solid transparent;
                            font-weight: normal;color: #428bca;padding: 1px 5px;font-size: 12px;line-height: 1.5;border-radius: 3px;
                            font-family:open_sansregular,Calibri,Arial,sans-serif" target="_blank" >
                      <xsl:attribute name="href">
                       {PORTAL_ROOT_PATH}/Esb/Fault?Init=True&amp;Status=UnResolved&amp;Application=<xsl:value-of select="ArrayOfAlertFaultSummary/AlertFaultSummary[1]/Application"/>&amp;FromDateTime=<xsl:value-of select="ArrayOfAlertFaultSummary/AlertFaultSummary[1]/MinTime"/>&amp;ToDateTime=<xsl:value-of select="ArrayOfAlertFaultSummary/AlertFaultSummary[1]/MaxTime"/>
                      </xsl:attribute>
                      View Application Faults
                    </a>

                  </li>
                  <li style="display: inline-block;padding-right: 5px;padding-left: 5px;">
                    <a style="display: inline-block;margin-bottom: 0;text-align: center;white-space: nowrap;vertical-align: middle;
                            -ms-touch-action: manipulation;touch-action: manipulation;cursor: pointer;-webkit-user-select: none;
                            -moz-user-select: none;-ms-user-select: none;user-select: none;background-image: none;border: 1px solid transparent;
                            font-weight: normal;color: #428bca;padding: 1px 5px;font-size: 12px;line-height: 1.5;border-radius: 3px;
                            font-family:open_sansregular,Calibri,Arial,sans-serif"
                       target="_blank">
                      <xsl:attribute name="href">
                       {PORTAL_ROOT_PATH}/Esb/Alert
                      </xsl:attribute>My Subscriptions
                    </a>
                  </li>
                </ul>
              </div>
            </div>
          </div>

        </footer>

      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
