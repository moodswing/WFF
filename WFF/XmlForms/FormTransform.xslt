<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" version="5.0" indent="yes" />
    <xsl:template name="controlForm">
        <xsl:param name="node" />
        <xsl:variable name="lowercase" select="'abcdefghijklmnopqrstuvwxyz'" />
        <xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />
        <xsl:variable name="userAssigned" select="translate(../../initialInput/userAssigned, $uppercase, $lowercase)" />
        <xsl:variable name="currentUser" select="translate(../../initialInput/currentUser, $uppercase, $lowercase)" />
        <xsl:variable name="requiredFields" select="../../statuses/status[name=../../initialInput/statusId]/requiredFields" />
        <xsl:variable name="editableFields" select="../../statuses/status[name=../../initialInput/statusId]/editableFields" />
        <xsl:variable name="isRequired" select="contains(concat(',', $requiredFields, ','), concat(',', $node/controlId, ','))" />
        <xsl:variable name="isEditable" select="$userAssigned = $currentUser and contains(concat(',', $editableFields, ','), concat(',', $node/controlId, ','))" />
        <xsl:variable name="parameters" select="../../parameters/parameter[id=$node/dataSource]" />
        <xsl:if test="$node/warning!=''">
            <div class="text-warning">
                <xsl:value-of select="warning" />
            </div>
        </xsl:if>
        <xsl:choose>
            <xsl:when test="$node/type='textbox'">
                <input type="textbox">
                    <xsl:attribute name="id">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:if test="$node/format != ''">
                        <xsl:attribute name="type">
                            <xsl:value-of select="$node/format" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$node/value != ''">
                        <xsl:attribute name="value">
                            <xsl:value-of select="$node/value" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:if test="not($isEditable)">
                        <xsl:attribute name="readonly">readonly</xsl:attribute>
                        <xsl:attribute name="class">disabled</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$isRequired">
                        <xsl:attribute name="data-val">true</xsl:attribute>
                        <xsl:attribute name="required">required</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                </input>
            </xsl:when>
            <xsl:when test="$node/type='date'">
                <div>
                    <xsl:if test="$isEditable">
                        <xsl:attribute name="class">input-group date</xsl:attribute>
                    </xsl:if>
                    <input type="textbox">
                        <xsl:if test="$isEditable">
                            <xsl:attribute name="class">form-control</xsl:attribute>
                        </xsl:if>
                        <xsl:attribute name="id">
                            <xsl:value-of select="$node/controlId" />
                        </xsl:attribute>
                        <xsl:attribute name="name">
                            <xsl:value-of select="$node/controlId" />
                        </xsl:attribute>
                        <xsl:if test="not($isEditable)">
                            <xsl:attribute name="readonly">readonly</xsl:attribute>
                            <xsl:attribute name="class">disabled</xsl:attribute>
                        </xsl:if>
                        <xsl:if test="$isRequired">
                            <xsl:attribute name="data-val">true</xsl:attribute>
                            <xsl:attribute name="required">required</xsl:attribute>
                        </xsl:if>
                        <xsl:if test="$node/visibleWhen">
                            <xsl:attribute name="data-visibleWhen">
                                <xsl:value-of select="$node/visibleWhen" />
                            </xsl:attribute>
                        </xsl:if>
                    </input>
                    <xsl:if test="$isEditable">
                        <span class="input-group-addon">
                            <i class="glyphicon glyphicon-th">
                            </i>
                        </span>
                    </xsl:if>
                </div>
            </xsl:when>
            <xsl:when test="$node/type='time'">
                <div>
                    <div data-placement="left" data-align="top" data-autoclose="true">
                        <xsl:if test="$isEditable">
                            <xsl:attribute name="class">clockpicker</xsl:attribute>
                        </xsl:if>
                        <xsl:if test="not($isEditable)">
                            <xsl:attribute name="class">clockpicker-readonly</xsl:attribute>
                        </xsl:if>
                        <input type="textbox" readonly="readonly">
                            <xsl:attribute name="id">
                                <xsl:value-of select="$node/controlId" />
                            </xsl:attribute>
                            <xsl:attribute name="name">
                                <xsl:value-of select="$node/controlId" />
                            </xsl:attribute>
                            <xsl:if test="not($isEditable)">
                                <xsl:attribute name="class">disabled</xsl:attribute>
                            </xsl:if>
                            <xsl:if test="$isRequired">
                                <xsl:attribute name="data-val">true</xsl:attribute>
                                <xsl:attribute name="required">required</xsl:attribute>
                            </xsl:if>
                            <xsl:if test="$node/visibleWhen">
                                <xsl:attribute name="data-visibleWhen">
                                    <xsl:value-of select="$node/visibleWhen" />
                                </xsl:attribute>
                            </xsl:if>
                        </input>
                        <xsl:if test="$isEditable">
                            <span class="input-group-addon">
                                <span class="glyphicon glyphicon-time">
                                </span>
                            </span>
                        </xsl:if>
                    </div>
                </div>
            </xsl:when>
            <xsl:when test="$node/type='label'">
                <input type="textbox" class="disabled">
                    <xsl:attribute name="id">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:attribute name="readonly">readonly</xsl:attribute>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:choose>
                        <xsl:when test="$node/dataOrigin != ''">
                            <xsl:choose>
                                <xsl:when test="$node/dataOrigin='initialInput'">
                                    <xsl:attribute name="value">
                                        <xsl:value-of select="//body/initialInput/*[local-name() = $node/value]" />
                                    </xsl:attribute>
                                </xsl:when>
                            </xsl:choose>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:attribute name="value">
                                <xsl:value-of select="$node/value" />
                            </xsl:attribute>
                        </xsl:otherwise>
                    </xsl:choose>
                </input>
                <xsl:if test="$node/alternativeValue = 'true'">
                    <input type="hidden">
                        <xsl:attribute name="name">
                            <xsl:value-of select="$node/controlId" />
                            <xsl:value-of select="$node/alternativeSuffix" />
                        </xsl:attribute>
                        <xsl:attribute name="id">
                            <xsl:value-of select="$node/controlId" />
                            <xsl:value-of select="$node/alternativeSuffix" />
                        </xsl:attribute>
                    </input>
                </xsl:if>
            </xsl:when>
            <xsl:when test="$node/type='radiobutton'">
                <div>
                    <xsl:attribute name="name">container<xsl:value-of select="$node/controlId" /></xsl:attribute>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:for-each select="$parameters">
                        <xsl:for-each select="options/option">
                            <label class="radiobutton">
                                <input type="radio">
                                    <xsl:if test="$isEditable">
                                        <xsl:attribute name="id">opt<xsl:value-of select="value" /></xsl:attribute>
                                    </xsl:if>
                                    <xsl:attribute name="name">
                                        <xsl:value-of select="$node/controlId" />
                                    </xsl:attribute>
                                    <xsl:attribute name="value">
                                        <xsl:value-of select="value" />
                                    </xsl:attribute>
                                    <xsl:if test="not($isEditable)">
                                        <xsl:attribute name="disabled">true</xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="default = 'true'">
                                        <xsl:attribute name="checked">true</xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="$isRequired">
                                        <xsl:attribute name="data-val">true</xsl:attribute>
                                        <xsl:attribute name="required">required</xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="hideControl != ''">
                                        <xsl:attribute name="data-hidden">
                                            <xsl:value-of select="hideControl" />
                                        </xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="removeControl != ''">
                                        <xsl:attribute name="data-hidden">
                                            <xsl:value-of select="removeControl" />
                                        </xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="count(changeFieldValues/changeValue) &gt; 0">
                                        <xsl:for-each select="changeFieldValues/changeValue">
                                            <xsl:attribute name="data-change{position()}">
                                                <xsl:value-of select="field" />|<xsl:value-of select="value" /></xsl:attribute>
                                        </xsl:for-each>
                                    </xsl:if>
                                    <xsl:if test="not($isEditable)">
                                        <input type="hidden">
                                            <xsl:attribute name="name">
                                                <xsl:value-of select="$node/controlId" />
                                            </xsl:attribute>
                                            <xsl:attribute name="id">opt<xsl:value-of select="value" /></xsl:attribute>
                                        </input>
                                    </xsl:if>
                                    <xsl:value-of select="value" />
                                </input>
                            </label>
                        </xsl:for-each>
                    </xsl:for-each>
                </div>
            </xsl:when>
            <xsl:when test="$node/type='checkbox'">
                <div>
                    <xsl:attribute name="name">container<xsl:value-of select="$node/controlId" /></xsl:attribute>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:for-each select="$parameters">
                        <xsl:for-each select="options/option">
                            <label class="check-box">
                                <input type="checkbox">
                                    <xsl:if test="$isEditable">
                                        <xsl:attribute name="id">opt<xsl:value-of select="value" /></xsl:attribute>
                                    </xsl:if>
                                    <xsl:attribute name="name">
                                        <xsl:value-of select="$node/controlId" />
                                    </xsl:attribute>
                                    <xsl:attribute name="value">
                                        <xsl:value-of select="value" />
                                    </xsl:attribute>
                                    <xsl:if test="not($isEditable)">
                                        <xsl:attribute name="disabled">true</xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="default = 'true'">
                                        <xsl:attribute name="checked">true</xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="$isRequired">
                                        <xsl:attribute name="data-val">true</xsl:attribute>
                                        <xsl:attribute name="required">required</xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="hideControl != ''">
                                        <xsl:attribute name="data-hidden">
                                            <xsl:value-of select="hideControl" />
                                        </xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="removeControl != ''">
                                        <xsl:attribute name="data-hidden">
                                            <xsl:value-of select="removeControl" />
                                        </xsl:attribute>
                                    </xsl:if>
                                    <xsl:if test="count(changeFieldValues/changeValue) &gt; 0">
                                        <xsl:for-each select="changeFieldValues/changeValue">
                                            <xsl:attribute name="data-change{position()}">
                                                <xsl:value-of select="field" />|<xsl:value-of select="value" /></xsl:attribute>
                                        </xsl:for-each>
                                    </xsl:if>
                                    <xsl:if test="not($isEditable)">
                                        <input type="hidden">
                                            <xsl:attribute name="name">
                                                <xsl:value-of select="$node/controlId" />
                                            </xsl:attribute>
                                            <xsl:attribute name="id">opt<xsl:value-of select="value" /></xsl:attribute>
                                        </input>
                                    </xsl:if>
                                    <xsl:value-of select="value" />
                                </input>
                            </label>
                        </xsl:for-each>
                    </xsl:for-each>
                </div>
            </xsl:when>
            <xsl:when test="$node/type='selectUser'">
                <input type="textbox" readonly="readonly">
                    <xsl:attribute name="id">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:if test="not($isEditable)">
                        <xsl:attribute name="class">disabled</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$isRequired">
                        <xsl:attribute name="data-val">true</xsl:attribute>
                        <xsl:attribute name="required">required</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                </input>
                <input type="hidden">
                    <xsl:attribute name="name">email<xsl:value-of select="$node/controlId" /></xsl:attribute>
                    <xsl:attribute name="id">email<xsl:value-of select="$node/controlId" /></xsl:attribute>
                </input>
                <xsl:if test="$isEditable">
                    <img class="btn-open-modal" src="../images/select-user.png" data-toggle="modal" data-target="#selectUserModal">
                        <xsl:attribute name="id">btnOpenSelectUser<xsl:value-of select="$node/controlId" /></xsl:attribute>
                    </img>
                </xsl:if>
            </xsl:when>
            <xsl:when test="$node/type='modalTable'">
                <input type="textbox" readonly="readonly">
                    <xsl:attribute name="id">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:if test="not($isEditable)">
                        <xsl:attribute name="class">disabled</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$isRequired">
                        <xsl:attribute name="data-val">true</xsl:attribute>
                        <xsl:attribute name="required">required</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:for-each select="$parameters/values/value[changeFieldValues]/..">
                        <xsl:for-each select="value/changeFieldValues/changeValue">
                            <xsl:attribute name="data-change{position()}">
                                <xsl:value-of select="../../col" />|<xsl:value-of select="field" />|<xsl:value-of select="value" /></xsl:attribute>
                        </xsl:for-each>
                    </xsl:for-each>
                </input>
                <xsl:if test="$isEditable">
                    <img class="btn-open-modal" src="../images/select-user.png" data-toggle="modal">
                        <xsl:attribute name="data-target">#modal<xsl:value-of select="$node/dataSource" /></xsl:attribute>
                        <xsl:attribute name="id">btnOpenModalParameter<xsl:value-of select="$node/dataSource" /></xsl:attribute>
                    </img>
                </xsl:if>
            </xsl:when>
            <xsl:when test="$node/type='rolesModalTable'">
                <input type="textbox" readonly="readonly">
                    <xsl:attribute name="id">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:if test="not($isEditable)">
                        <xsl:attribute name="class">disabled</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$isRequired">
                        <xsl:attribute name="data-val">true</xsl:attribute>
                        <xsl:attribute name="required">required</xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                </input>
                <input type="hidden">
                    <xsl:attribute name="name">email<xsl:value-of select="$node/controlId" /></xsl:attribute>
                    <xsl:attribute name="id">email<xsl:value-of select="$node/controlId" /></xsl:attribute>
                </input>
                <xsl:if test="$isEditable">
                    <img class="btn-open-modal" src="../images/select-user.png" data-toggle="modal">
                        <xsl:attribute name="data-target">#modal<xsl:value-of select="$node/dataSource" /></xsl:attribute>
                        <xsl:attribute name="id">btnOpenModalParameter<xsl:value-of select="$node/dataSource" /></xsl:attribute>
                    </img>
                </xsl:if>
            </xsl:when>
            <xsl:when test="$node/type='files'">
                <xsl:variable name="attachments" select="../../initialInput/attachments/attachment[field=$node/controlId]" />
                <xsl:variable name="numberOfAttachments" select="count($attachments/file)" />
                <xsl:if test="$numberOfAttachments &gt; 0">
                    <xsl:for-each select="$attachments/file">
                        <xsl:variable name="position" select="position() - 1" />
                        <div>
                            <xsl:attribute name="id">container<xsl:value-of select="concat($node/controlId, ':', $position)" /></xsl:attribute>
                            <xsl:attribute name="name">container<xsl:value-of select="concat($node/controlId, ':', $position)" /></xsl:attribute>
                            <input type="textbox" class="txt-file disabled" readonly="readonly">
                                <xsl:attribute name="id">
                                    <xsl:value-of select="concat($node/controlId, ':', $position)" />
                                </xsl:attribute>
                                <xsl:attribute name="name">
                                    <xsl:value-of select="concat($node/controlId, ':', $position)" />
                                </xsl:attribute>
                                <xsl:attribute name="value">
                                    <xsl:value-of select="fileName" />
                                </xsl:attribute>
                                <xsl:if test="$isRequired">
                                    <xsl:attribute name="data-val">true</xsl:attribute>
                                    <xsl:attribute name="required">required</xsl:attribute>
                                </xsl:if>
                            </input>
                            <img class="btn-open-file" alt="open" src="../images/download.png">
                                <xsl:attribute name="id">btnOpenFile<xsl:value-of select="concat($node/controlId, ':', $position)" /></xsl:attribute>
                                <xsl:attribute name="onclick">window.open('<xsl:value-of select="location" />');</xsl:attribute>
                            </img>
                            <xsl:if test="$isEditable">
                                <img class="btn-delete-file" alt="delete" src="../images/delete.png">
                                    <xsl:attribute name="id">btnDeleteFile<xsl:value-of select="concat($node/controlId, ':', $position)" /></xsl:attribute>
                                    <xsl:attribute name="onclick">removeModal('<xsl:value-of select="concat($node/controlId, ':', $position)" />');</xsl:attribute>
                                </img>
                            </xsl:if>
                            <input type="file" style="display: none;">
                                <xsl:attribute name="id">file<xsl:value-of select="concat($node/controlId, ':', $position)" /></xsl:attribute>
                                <xsl:attribute name="name">file<xsl:value-of select="concat($node/controlId, ':', $position)" /></xsl:attribute>
                            </input>
                            <input type="hidden">
                                <xsl:attribute name="name">fileId<xsl:value-of select="$node/controlId" /></xsl:attribute>
                                <xsl:attribute name="id">fileId<xsl:value-of select="$node/controlId" /></xsl:attribute>
                                <xsl:attribute name="value">
                                    <xsl:value-of select="id" />
                                </xsl:attribute>
                            </input>
                        </div>
                    </xsl:for-each>
                </xsl:if>
                <div style="display:none;" class="disabled">
                    <xsl:attribute name="id">container<xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" /></xsl:attribute>
                    <xsl:attribute name="name">container<xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" /></xsl:attribute>
                    <input type="textbox" class="txt-file" readonly="readonly">
                        <xsl:attribute name="id">
                            <xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" /></xsl:attribute>
                        <xsl:attribute name="name">
                            <xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" /></xsl:attribute>
                        <xsl:if test="$isRequired">
                            <xsl:attribute name="data-val">true</xsl:attribute>
                            <xsl:attribute name="required">required</xsl:attribute>
                        </xsl:if>
                    </input>
                    <xsl:if test="$isEditable">
                        <img class="btn-delete-file" alt="delete" src="../images/delete.png">
                            <xsl:attribute name="id">btnDeleteFile<xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" /></xsl:attribute>
                            <xsl:attribute name="onclick">removeModal('<xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" />');</xsl:attribute>
                        </img>
                    </xsl:if>
                    <input type="file" style="display: none;">
                        <xsl:attribute name="id">file<xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" /></xsl:attribute>
                        <xsl:attribute name="name">file<xsl:value-of select="$node/controlId" />:<xsl:value-of select="$numberOfAttachments" /></xsl:attribute>
                    </input>
                </div>
                <xsl:if test="$isEditable">
                    <img class="btn-file" src="../images/file.png">
                        <xsl:attribute name="id">btnSelectFile<xsl:value-of select="$node/controlId" /></xsl:attribute>
                        <xsl:attribute name="onclick">openFileModal('<xsl:value-of select="$node/controlId" />');</xsl:attribute>
                    </img>
                </xsl:if>
            </xsl:when>
            <xsl:when test="$node/type='list'">
                <xsl:variable name="lists" select="../../initialInput/lists/list[field=$node/controlId]" />
                <div style="display:none;">
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                    <xsl:attribute name="id">container<xsl:value-of select="$node/controlId" />:0</xsl:attribute>
                    <xsl:attribute name="name">container<xsl:value-of select="$node/controlId" />:0</xsl:attribute>
                    <div class="input-list disabled">
                        <xsl:attribute name="name">
                            <xsl:value-of select="$node/controlId" />:0</xsl:attribute>
                        <xsl:if test="position() = count($lists/item)">
                            <xsl:attribute name="class">input-list last disabled</xsl:attribute>
                        </xsl:if>
                    </div>
                    <input type="hidden" readonly="readonly">
                        <xsl:attribute name="id">
                            <xsl:value-of select="$node/controlId" />:0</xsl:attribute>
                        <xsl:attribute name="name">
                            <xsl:value-of select="$node/controlId" />:0</xsl:attribute>
                    </input>
                </div>
            </xsl:when>
            <xsl:when test="$node/type='custom'">
                <table class="table custom-field">
                    <xsl:attribute name="id">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:value-of select="$node/controlId" />
                    </xsl:attribute>
                    <xsl:if test="$node/visibleWhen">
                        <xsl:attribute name="data-visibleWhen">
                            <xsl:value-of select="$node/visibleWhen" />
                        </xsl:attribute>
                    </xsl:if>
                    <thead>
                        <tr>
                            <xsl:for-each select="$node/details/field">
                                <th scope="col">
                                    <xsl:value-of select="label" />
                                </th>
                            </xsl:for-each>
                        </tr>
                    </thead>
                    <tbody>
                        <tr scope="row">
                            <xsl:for-each select="$node/details/field">
                                <td>
                                    <xsl:choose>
                                        <xsl:when test="type='textbox'">
                                            <input type="textbox">
                                                <xsl:attribute name="id">
                                                    <xsl:value-of select="controlId" />
                                                </xsl:attribute>
                                                <xsl:attribute name="name">
                                                    <xsl:value-of select="controlId" />
                                                </xsl:attribute>
                                                <xsl:if test="$node/value != ''">
                                                    <xsl:attribute name="value">
                                                        <xsl:value-of select="$node/value" />
                                                    </xsl:attribute>
                                                </xsl:if>
                                                <xsl:if test="not($isEditable)">
                                                    <xsl:attribute name="readonly">readonly</xsl:attribute>
                                                    <xsl:attribute name="class">disabled</xsl:attribute>
                                                </xsl:if>
                                                <xsl:if test="$isRequired">
                                                    <xsl:attribute name="data-val">true</xsl:attribute>
                                                    <xsl:attribute name="required">required</xsl:attribute>
                                                </xsl:if>
                                            </input>
                                        </xsl:when>
                                    </xsl:choose>
                                </td>
                            </xsl:for-each>
                        </tr>
                    </tbody>
                    <input type="hidden">
                        <xsl:attribute name="name">
                            <xsl:value-of select="$node/controlId" />
                        </xsl:attribute>
                        <xsl:attribute name="id">
                            <xsl:value-of select="$node/controlId" />
                        </xsl:attribute>
                    </input>
                </table>
            </xsl:when>
            <xsl:when test="$node/type='customList'">
                <div class="custom-list-field">
                    <table class="table">
                        <xsl:attribute name="id">
                            <xsl:value-of select="$node/controlId" />
                        </xsl:attribute>
                        <xsl:attribute name="name">
                            <xsl:value-of select="$node/controlId" />
                        </xsl:attribute>
                        <xsl:if test="$node/visibleWhen">
                            <xsl:attribute name="data-visibleWhen">
                                <xsl:value-of select="$node/visibleWhen" />
                            </xsl:attribute>
                        </xsl:if>
                        <thead>
                            <tr>
                                <xsl:for-each select="$node/details/field">
                                    <th scope="col">
                                        <xsl:value-of select="label" />
                                    </th>
                                </xsl:for-each>
                            </tr>
                        </thead>
                        <tbody>
                            <tr scope="row">
                                <xsl:attribute name="id">
                                    <xsl:value-of select="controlId" />Row:0</xsl:attribute>
                                <xsl:attribute name="name">
                                    <xsl:value-of select="controlId" />Row:0</xsl:attribute>
                                <xsl:for-each select="$node/details/field">
                                    <td>
                                        <xsl:choose>
                                            <xsl:when test="type='textbox'">
                                                <input type="textbox">
                                                    <xsl:attribute name="id">
                                                        <xsl:value-of select="controlId" />:0</xsl:attribute>
                                                    <xsl:attribute name="name">
                                                        <xsl:value-of select="controlId" />:0</xsl:attribute>
                                                    <xsl:if test="not($isEditable)">
                                                        <xsl:attribute name="readonly">readonly</xsl:attribute>
                                                        <xsl:attribute name="class">disabled</xsl:attribute>
                                                    </xsl:if>
                                                    <xsl:if test="$isRequired">
                                                        <xsl:attribute name="data-val">true</xsl:attribute>
                                                        <xsl:attribute name="required">required</xsl:attribute>
                                                    </xsl:if>
                                                </input>
                                            </xsl:when>
                                        </xsl:choose>
                                    </td>
                                </xsl:for-each>
                                <td class="delete-item">
                                    <img class="btn-delete-file" alt="delete" src="../images/delete.png">
                                        <xsl:attribute name="id">btnDeleteFile<xsl:value-of select="$node/controlId" />:0</xsl:attribute>
                                        <xsl:attribute name="onclick">removeCustomListItemModal('<xsl:value-of select="$node/controlId" />Row:0')</xsl:attribute>
                                    </img>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="add-item">
                        <a href="#">
                            <xsl:value-of select="$node/addLabel" />
                        </a>
                    </div>
                </div>
            </xsl:when>
        </xsl:choose>
    </xsl:template>
    <xsl:template match="text()" name="split">
        <xsl:param name="pText" select="." />
        <xsl:if test="string-length($pText)">
            <xsl:value-of select="substring-before(concat($pText,'\n'),'\n')" />
            <br />
            <xsl:call-template name="split">
                <xsl:with-param name="pText" select="substring-after($pText, '\n')" />
            </xsl:call-template>
        </xsl:if>
    </xsl:template>
    <xsl:template match="body">
        <link rel="stylesheet" type="text/css" href="../css/form-request.css" />
        <script src="../js/form-request.js" />
        <div class="container form-container">
            <form id="requestForm">
                <div>
                    <input type="hidden">
                        <xsl:attribute name="name">FormId</xsl:attribute>
                        <xsl:attribute name="id">FormId</xsl:attribute>
                        <xsl:attribute name="value">
                            <xsl:value-of select="idForm" />
                        </xsl:attribute>
                    </input>
                </div>
                <h1>
                    <xsl:value-of select="headerTitle" />
                </h1>
                <h4>
                    <xsl:value-of select="headerSubTitle" />
                </h4>
                <ul class="nav nav-tabs">
                    <xsl:for-each select="pages/page">
                        <li class="nav-item">
                            <xsl:if test="position()=1">
                                <xsl:attribute name="class">nav-item active</xsl:attribute>
                            </xsl:if>
                            <a class="nav-link" href="#" data-toggle="tab" role="tab" aria-selected="false">
                                <xsl:if test="position()=1">
                                    <xsl:attribute name="aria-selected">true</xsl:attribute>
                                </xsl:if>
                                <xsl:attribute name="href">
                  #tab<xsl:value-of select="id" /></xsl:attribute>
                                <xsl:attribute name="aria-controls">
                  tab<xsl:value-of select="id" /></xsl:attribute>
                                <xsl:value-of select="label" />
                            </a>
                        </li>
                    </xsl:for-each>
                </ul>
                <div class="tab-content form-request">
                    <div class="row header">
                        <div class="status">
              Status: <xsl:value-of select="initialInput/status" /></div>
                    </div>
                    <xsl:for-each select="pages/page">
                        <xsl:sort select="page" />
                        <xsl:variable name="idPage" select="id" />
                        <xsl:variable name="requiredFields" select="../../statuses/status[name=../../initialInput/statusId]/requiredFields" />
                        <xsl:variable name="hiddenFields" select="../../statuses/status[name=../../initialInput/statusId]/hiddenFields" />
                        <div class="tab-pane fade" role="tabpanel">
                            <xsl:if test="position()=1">
                                <xsl:attribute name="class">tab-pane fade active in</xsl:attribute>
                            </xsl:if>
                            <!-- attributes must be in just one line or bootstrap fails -->
                            <xsl:attribute name="href">tab<xsl:value-of select="$idPage" /></xsl:attribute>
                            <xsl:attribute name="aria-labelledby">tab<xsl:value-of select="$idPage" /></xsl:attribute>
                            <xsl:attribute name="id">tab<xsl:value-of select="$idPage" /></xsl:attribute>
                            <div class="container tab-container">
                                <div class="row">
                                    <xsl:for-each select="../../fields/field[page = $idPage]">
                                        <xsl:variable name="index" select="position()" />
                                        <xsl:variable name="isRequired" select="contains(concat(',', $requiredFields, ','), concat(',', controlId, ','))" />
                                        <xsl:variable name="followingNode" select="following-sibling::field[1]" />
                                        <xsl:variable name="precedingNode" select="preceding-sibling::field[1]" />
                                        <xsl:variable name="isNextFieldRequired" select="contains(concat(',', $requiredFields, ','), concat(',', $followingNode/controlId, ','))" />
                                        <xsl:variable name="isDisplayed" select="not(contains(concat(',', $hiddenFields, ','), concat(',', controlId, ',')))" />
                                        <xsl:variable name="isFollowingDisplayed" select="not(contains(concat(',', $hiddenFields, ','), concat(',', $followingNode/controlId, ',')))" />
                                        <xsl:variable name="fullrow-length">
                                            <xsl:choose>
                                                <xsl:when test="halfRowLabel">
                                                    <xsl:text>col-sm-6 full-row</xsl:text>
                                                </xsl:when>
                                                <xsl:when test="label != ''">
                                                    <xsl:text>col-sm-10 full-row</xsl:text>
                                                </xsl:when>
                                                <xsl:otherwise>
                                                    <xsl:text>col-sm-12 full-row</xsl:text>
                                                </xsl:otherwise>
                                            </xsl:choose>
                                        </xsl:variable>
                                        <xsl:choose>
                                            <xsl:when test="($isDisplayed and columnSize='full') or type = 'hidden'">
                                                <xsl:choose>
                                                    <xsl:when test="type = 'hidden'">
                                                        <div>
                                                            <input type="hidden">
                                                                <xsl:attribute name="name">
                                                                    <xsl:value-of select="controlId" />
                                                                </xsl:attribute>
                                                                <xsl:attribute name="id">
                                                                    <xsl:value-of select="controlId" />
                                                                </xsl:attribute>
                                                                <xsl:if test="value">
                                                                    <xsl:attribute name="value">
                                                                        <xsl:value-of select="value" />
                                                                    </xsl:attribute>
                                                                </xsl:if>
                                                            </input>
                                                        </div>
                                                    </xsl:when>
                                                    <xsl:otherwise>
                                                        <div class="row full-row col-sm-12">
                                                            <xsl:if test="visible">
                                                                <xsl:attribute name="style">display:none</xsl:attribute>
                                                            </xsl:if>
                                                            <xsl:if test="label != ''">
                                                                <div class="col-sm-2">
                                                                    <xsl:if test="halfRowLabel">
                                                                        <xsl:attribute name="class">col-sm-6 full-row</xsl:attribute>
                                                                    </xsl:if>
                                                                    <xsl:if test="$isRequired">
                                                                        <div class="required-field">*</div>
                                                                    </xsl:if>
                                                                    <label>
                                                                        <xsl:value-of select="label" />
                                                                    </label>
                                                                    <xsl:if test="description!=''">
                                                                        <div class="description">
                                                                            <xsl:value-of select="description" disable-output-escaping="yes" />
                                                                        </div>
                                                                    </xsl:if>
                                                                </div>
                                                            </xsl:if>
                                                            <div>
                                                                <xsl:attribute name="class">
                                                                    <xsl:value-of select="$fullrow-length" />
                                                                </xsl:attribute>
                                                                <xsl:call-template name="controlForm">
                                                                    <xsl:with-param name="node" select="." />
                                                                </xsl:call-template>
                                                            </div>
                                                        </div>
                                                    </xsl:otherwise>
                                                </xsl:choose>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <div class="row col-sm-6">
                                                    <xsl:if test="(visible = 'false' or not($isDisplayed)) and $followingNode/controlId = 'Empty'">
                                                        <xsl:attribute name="style">display:none</xsl:attribute>
                                                    </xsl:if>
                                                    <xsl:if test="($followingNode/visible = 'false' or not($isFollowingDisplayed)) and controlId = 'Empty'">
                                                        <xsl:attribute name="style">display:none</xsl:attribute>
                                                    </xsl:if>
                                                    <xsl:if test="(visible = 'false' or not($isDisplayed)) and $followingNode/controlId = 'Empty' and controlId != 'Empty' and replacingField = 'true'">
                                                        <xsl:attribute name="class">row replacing-field</xsl:attribute>
                                                    </xsl:if>
                                                    <xsl:if test="($followingNode/visible = 'false' or not($isFollowingDisplayed)) and controlId = 'Empty' and $followingNode/controlId != 'Empty' and $followingNode/replacingField = 'true'">
                                                        <xsl:attribute name="class">row replacing-field</xsl:attribute>
                                                    </xsl:if>
                                                    <xsl:if test="visible = 'false' or not($isDisplayed)">
                                                        <xsl:attribute name="style">display:none</xsl:attribute>
                                                    </xsl:if>
                                                    <div class="half-row">
                                                        <xsl:if test="controlId='Empty'">
                                                            <xsl:attribute name="class">half-row empty</xsl:attribute>
                                                        </xsl:if>
                                                        <div class="col-sm-4">
                                                            <xsl:if test="$isRequired">
                                                                <div class="required-field">*</div>
                                                            </xsl:if>
                                                            <label>
                                                                <xsl:value-of select="label" />
                                                            </label>
                                                            <xsl:if test="description!=''">
                                                                <div class="description">
                                                                    <xsl:value-of select="description" disable-output-escaping="yes" />
                                                                </div>
                                                            </xsl:if>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <xsl:call-template name="controlForm">
                                                                <xsl:with-param name="node" select="." />
                                                            </xsl:call-template>
                                                        </div>
                                                    </div>
                                                </div>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </xsl:for-each>
                                </div>
                            </div>
                        </div>
                    </xsl:for-each>
                </div>
                <div>
                    <xsl:for-each select="initialInput/*[not(*)]">
                        <input type="hidden">
                            <xsl:attribute name="name">
                                <xsl:value-of select="name(.)" />
                            </xsl:attribute>
                            <xsl:attribute name="id">
                                <xsl:value-of select="name(.)" />
                            </xsl:attribute>
                            <xsl:attribute name="value">
                                <xsl:value-of select="." />
                            </xsl:attribute>
                        </input>
                    </xsl:for-each>
                </div>
            </form>
            <div class="row form-request-footer">
                <div class="status">
              Created By: <xsl:value-of select="initialInput/createdBy" /><br />
              Created Date: <xsl:value-of select="initialInput/createdDate" /></div>
            </div>
        </div>
        <div>
            <xsl:for-each select="parameters/parameter[type='table']">
                <div class="modal fade" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                    <xsl:attribute name="id">modal<xsl:value-of select="id" /></xsl:attribute>
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content" style="width: 600px;">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">x</span>
                                </button>
                                <h3 class="modal-title" id="exampleModalLongTitle">
                                    <xsl:value-of select="title" />
                                </h3>
                            </div>
                            <xsl:if test="subtitle != ''">
                                <div class="modal-body">
                                    <xsl:value-of select="subtitle" />
                                </div>
                            </xsl:if>
                            <div class="modal-table-container">
                                <table class="table table-hover table-clickable">
                                    <thead>
                                        <tr>
                                            <xsl:for-each select="columns/column">
                                                <th scope="col">
                                                    <xsl:if test="@saveValue = 'true'">
                                                        <xsl:attribute name="save">true</xsl:attribute>
                                                    </xsl:if>
                                                    <xsl:value-of select="." />
                                                </th>
                                            </xsl:for-each>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <xsl:for-each select="values/value">
                                            <tr class="clickable-row" scope="row">
                                                <xsl:for-each select="col">
                                                    <td>
                                                        <xsl:value-of select="." />
                                                    </td>
                                                </xsl:for-each>
                                            </tr>
                                        </xsl:for-each>
                                    </tbody>
                                </table>
                            </div>
                            <div class="modal-footer row">
                                <div class="col-sm-7 validation-error">
                  Please select an option first
                </div>
                                <div class="col-sm-5">
                                    <input type="hidden">
                                        <xsl:attribute name="id">modalCaller<xsl:value-of select="id" /></xsl:attribute>
                                    </input>
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                    <button name="btnSelectOption" type="button" class="btn btn-primary">Select</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </xsl:for-each>
        </div>
        <div>
            <xsl:for-each select="roles/role[usedForModalTable='true']">
                <div class="modal fade" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
                    <xsl:attribute name="id">modal<xsl:value-of select="id" /></xsl:attribute>
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content" style="width: 600px;">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">x</span>
                                </button>
                                <h3 class="modal-title" id="exampleModalLongTitle">
                                    <xsl:value-of select="title" />
                                </h3>
                            </div>
                            <xsl:if test="subtitle != ''">
                                <div class="modal-body">
                                    <xsl:value-of select="subtitle" />
                                </div>
                            </xsl:if>
                            <div class="modal-table-container">
                                <table class="table table-hover table-clickable">
                                    <thead>
                                        <tr>
                                            <th scope="col" save="true">
                        User
                      </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <xsl:for-each select="users/user">
                                            <tr class="clickable-row" scope="row">
                                                <td>
                                                    <xsl:attribute name="data-email">
                                                        <xsl:value-of select="email" />
                                                    </xsl:attribute>
                                                    <xsl:value-of select="name" />
                                                </td>
                                            </tr>
                                        </xsl:for-each>
                                    </tbody>
                                </table>
                            </div>
                            <div class="modal-footer row">
                                <div class="col-sm-7 validation-error">
                  Please select an option first
                </div>
                                <div class="col-sm-5">
                                    <input type="hidden">
                                        <xsl:attribute name="id">modalCaller<xsl:value-of select="id" /></xsl:attribute>
                                    </input>
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                                    <button name="btnSelectOption" type="button" class="btn btn-primary">Select</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </xsl:for-each>
        </div>
    </xsl:template>
</xsl:stylesheet>